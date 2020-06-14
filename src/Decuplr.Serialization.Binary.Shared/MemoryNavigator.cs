using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Decuplr.Serialization.Binary {
    public ref struct MemoryNavigator<T> where T : unmanaged {

        private readonly ReadOnlySequence<T> Sequence;
        private SequencePosition NextPosition;
        private bool HasMoreData;
        private long _length;

        public ReadOnlySpan<T> CurrentSpan { get; private set; }

        public int CurrentSpanIndex { get; private set; }

        public long Consumed { get; private set; }

        public readonly long Remaining => Length - Consumed;

        public readonly long Length { 
            get {
                if (_length < 0)
                    Volatile.Write(ref Unsafe.AsRef(_length), Sequence.Length);
                return _length;
            } 
        }

        public readonly ReadOnlySpan<T> UnreadSpan => CurrentSpan.Slice(CurrentSpanIndex);

        public bool End => !HasMoreData;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryNavigator(ReadOnlySequence<T> sequence) {
            CurrentSpanIndex = 0;
            Consumed = 0;
            _length = -1;

            Sequence = sequence;
            NextPosition = sequence.Start;
            sequence.TryGet(ref NextPosition, out var firstMemory, true);

            CurrentSpan = firstMemory.Span;
            HasMoreData = firstMemory.Length <= 0;

            if (!HasMoreData && !sequence.IsSingleSegment) {
                HasMoreData = true;
                GetNextSpan();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryNavigator(ReadOnlySpan<T> span) {
            CurrentSpan = span;
            HasMoreData = false;

            Sequence = default;
            NextPosition = default;
            CurrentSpanIndex = 0;
            Consumed = 0;
            _length = span.Length;
        }

        private void GetNextSpan() {
            if (!Sequence.IsSingleSegment) {
                while (Sequence.TryGet(ref NextPosition, out ReadOnlyMemory<T> memory, advance: true)) {
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

        private readonly bool TryCopyMultisegment(Span<T> destination) {
            // If we don't have enough to fill the requested buffer, return false
            if (Remaining < destination.Length)
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
        /// Move the reader ahead the specified number of items.
        /// </summary>
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
