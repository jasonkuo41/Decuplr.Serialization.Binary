using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Decuplr.Serialization.Binary {

    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT license.
    // See the LICENSE file in the project root for more information.

    // This is a dumb down version of SequenceReader<T>, you cannot rewind with cursor, it's one use and should always move on
    // Lots of the surface API is stripped or modified to emphasize the concept of "it's a cursor not a reader"
    // This is passed around the library representing how much we have read from the sequence.

    // TODO : Should we move this Decuplr.Serialization.Primitives under the namespace Decuplr.Serialization?

    /// <summary>
    /// Represents a forward reading cursor for <see cref="ReadOnlySequence{T}"/> that cannot be rewinded
    /// </summary>
    /// <typeparam name="T">The type that the sequence holds</typeparam>
    public ref struct SequenceCursor<T> {

        private readonly ReadOnlySequence<T> Sequence;
        private readonly long _length;
        private SequencePosition CurrentPosition;
        private SequencePosition NextPosition;
        private bool HasMoreData;
        private ReadOnlySpan<T> CurrentSpan;
        private int CurrentSpanIndex;

        /// <summary>
        /// Represents how much data has been consumed from the sequence.
        /// </summary>
        public long Consumed { get; private set; }

        /// <summary>
        /// The position that the cursor is currently poiting to, can be considered as how much has consumed
        /// </summary>
        public readonly SequencePosition Position => Sequence.GetPosition(CurrentSpanIndex, CurrentPosition);

        /// <summary>
        /// Represents the total length of the source sequence.
        /// </summary>
        public readonly long Length { 
            get {
                if (_length < 0)
                    Volatile.Write(ref Unsafe.AsRef(_length), Sequence.Length);
                return _length;
            } 
        }

        /// <summary>
        /// Represents the remaining unread span of the current segment from the source sequence.
        /// </summary>
        public readonly ReadOnlySpan<T> UnreadSpan => CurrentSpan.Slice(CurrentSpanIndex);

        /// <summary>
        /// Indicates that there is no more data after this cursor.
        /// </summary>
        public bool Completed => !HasMoreData;

        /// <summary>
        /// Creates a <see cref="SequenceCursor{T}"/> over a given <see cref="ReadOnlySequence{T}"/>
        /// </summary>
        /// <param name="sequence">The read-only sequence that <see cref="SequenceCursor{T}"/> navigates with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Avoid copying the sequence twice, we just need one copy
        public SequenceCursor(in ReadOnlySequence<T> sequence) {
            CurrentSpanIndex = 0;
            Consumed = 0;
            _length = -1;

            Sequence = sequence;
            CurrentPosition = sequence.Start;
            NextPosition = sequence.Start;
            sequence.TryGet(ref NextPosition, out var firstMemory, true);

            CurrentSpan = firstMemory.Span;
            HasMoreData = firstMemory.Length > 0;

            if (!HasMoreData && !sequence.IsSingleSegment) {
                HasMoreData = true;
                GetNextSpan();
            }
        }

        /// <summary>
        /// Creates a <see cref="SequenceCursor{T}"/> over a given <see cref="ReadOnlySequence{T}"/>
        /// </summary>
        /// <param name="sequence">The read-only sequence that <see cref="SequenceCursor{T}"/> navigates with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SequenceCursor(ReadOnlySequence<T> sequence) : this(in sequence) { }

        private void GetNextSpan() {
            if (!Sequence.IsSingleSegment) {
                SequencePosition previousNextPosition = NextPosition;
                while (Sequence.TryGet(ref NextPosition, out ReadOnlyMemory<T> memory, advance: true)) {
                    CurrentPosition = previousNextPosition;
                    if (memory.Length > 0) {
                        CurrentSpan = memory.Span;
                        CurrentSpanIndex = 0;
                        return;
                    }
                    else {
                        CurrentSpan = default;
                        CurrentSpanIndex = 0;
                    }
                }
            }
            HasMoreData = false;
        }

        /// <summary>
        /// Tries to copy a segment of the data to the destination span.
        /// </summary>
        /// <param name="destination">The span to copy to</param>
        /// <returns>If the sequence is large enough to fully copy to the span</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(Span<T> destination) {
            // This API doesn't advance to facilitate conditional advancement based on the data returned.
            // We don't provide an advance option to allow easier utilizing of stack allocated destination spans.
            // (Because we can make this method readonly we can guarantee that we won't capture the span.)

            ReadOnlySpan<T> firstSpan = UnreadSpan;
            if (firstSpan.Length >= destination.Length) {
                firstSpan.Slice(0, destination.Length).CopyTo(destination);
                return true;
            }

            // Not enough in the current span to satisfy the request, fall through to the slow path
            return TryCopyMultisegment(destination);
        }

        /// <summary>
        /// Tries to copy mutliple segment of data to the destination span.
        /// </summary>
        /// <param name="destination">The span to copy to</param>
        /// <returns>If the sequence is large enough to fully copy to span</returns>
        internal readonly bool TryCopyMultisegment(Span<T> destination) {
            // If we don't have enough to fill the requested buffer, return false
            if (Length - Consumed < destination.Length)
                return false;

            ReadOnlySpan<T> firstSpan = UnreadSpan;
            Debug.Assert(firstSpan.Length < destination.Length);
            firstSpan.CopyTo(destination);
            int copied = firstSpan.Length;

            SequencePosition next = NextPosition;
            while (Sequence.TryGet(ref next, out ReadOnlyMemory<T> nextSegment, true)) {
                if (nextSegment.Length > 0) {
                    ReadOnlySpan<T> nextSpan = nextSegment.Span;
                    int toCopy = Math.Min(nextSpan.Length, destination.Length - copied);
                    nextSpan.Slice(0, toCopy).CopyTo(destination.Slice(copied));
                    copied += toCopy;
                    if (copied >= destination.Length) {
                        break;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Move the cursor ahead the specified number of items.
        /// </summary>
        // Possible optimization point : TryAdvance(Span<byte> data);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(long count) {
            const long TooBigOrNegative = unchecked((long)0xFFFFFFFF80000000);
            if ((count & TooBigOrNegative) == 0 && CurrentSpan.Length - CurrentSpanIndex > (int)count) {
                CurrentSpanIndex += (int)count;
                Consumed += count;
            }
            else {
                // Can't satisfy from the current span
                AdvanceToNextSpan(count);
            }
        }

        private void AdvanceToNextSpan(long count) {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Consumed += count;
            while (HasMoreData) {
                int remaining = CurrentSpan.Length - CurrentSpanIndex;

                if (remaining > count) {
                    CurrentSpanIndex += (int)count;
                    count = 0;
                    break;
                }

                // As there may not be any further segments we need to
                // push the current index to the end of the span.
                CurrentSpanIndex += remaining;
                count -= remaining;
                Debug.Assert(count >= 0);

                GetNextSpan();

                if (count == 0) {
                    break;
                }
            }

            if (count != 0) {
                // Not enough data left- adjust for where we actually ended and throw
                Consumed -= count;
                throw new ArgumentOutOfRangeException(nameof(count));
            }
        }

    }

}
