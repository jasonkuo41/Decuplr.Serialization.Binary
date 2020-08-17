using System;
using System.Buffers;
using Decuplr.Serialization.Binary.Utility;

namespace Decuplr.Serialization.Binary {

    /// <summary>
    /// Provides the base class for serializing and deserializing <typeparamref name="T"/> objects into it's binary counterpart
    /// </summary>
    /// <typeparam name="T">The target type for serialization and deserialization</typeparam>
    public abstract class BinaryConverter<T> {

        public void Serialize(in T item, IBufferWriter<byte> data) {
            var structBuffer = new StructBufferWriterWrap<byte>(data ?? throw new ArgumentNullException(nameof(data)));
            Serialize(item, ref structBuffer);
        }

        public void Serialize<TWriter>(in T item, ref TWriter writer) where TWriter : struct, IBufferWriter<byte>
            => Serialize(item, new NoopWriteState<T>(), ref writer);

        public virtual void Serialize<TState, TWriter>(in T item, in TState state, ref TWriter writer)
            where TState : struct, IBinaryWriteState<TState, T>
            where TWriter : struct, IBufferWriter<byte> {
            var length = GetSpanLength(item);
            var span = writer.GetSpan(length);
            var writtenBytes = Serialize(item, span);
            writer.Advance(writtenBytes);
        }

        public int GetSpanLength(in T item) => GetSpanLength(item, new NoopWriteState<T>());

        public abstract int GetSpanLength<TState>(in T item, in TState writeState) where TState : struct, IBinaryWriteState<TState, T>;

        public int Serialize(in T item, Span<byte> data) => Serialize(item, new NoopWriteState<T>(), data);

        public abstract int Serialize<TState>(in T item, in TState writeState, Span<byte> data) where TState : struct, IBinaryWriteState<TState, T>;

        public abstract SpanReadResult<T> Deserialize(ReadOnlySpan<byte> data);

        public SequenceReadResult<T> Deserialize(ReadOnlySequence<byte> sequence) {
            // Create cursor for this sequence
            var cursor = new SequenceCursor<byte>(in sequence);
            var result = Deserialize(ref cursor);
            return new SequenceReadResult<T>(result, cursor.Consumed, cursor.Position);
        }

        public abstract T Deserialize(ref SequenceCursor<byte> cursor);

        public int GetBlockLength(ReadOnlySequence<byte> sequence) {
            var cursor = new SequenceCursor<byte>(in sequence);
            return GetBlockLength(ref cursor);
        }

        public abstract int GetBlockLength(ReadOnlySpan<byte> data);

        public abstract int GetBlockLength(ref SequenceCursor<byte> cursor);
    }

}
