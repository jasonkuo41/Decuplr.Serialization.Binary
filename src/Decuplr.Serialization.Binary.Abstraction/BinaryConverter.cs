using System;
using System.Buffers;

namespace Decuplr.Serialization.Binary {
    public abstract class BinaryConverter<T> {

        public byte[] Serialize(in T item) {
            using var buffer = new PooledByteBufferWriter(256);
            Serialize(item, buffer);
            return buffer.WrittenMemory.ToArray();
        }

        // This part of the code should perform better then IBufferWriter<byte>
#if SHAPE_BUFFER_WRITER
        public virtual void Serialize<TWriter>(in T item, ref TWriter writer) where TWriter : struct, IBufferWriter<byte> {
            var length = GetSpanLength(item);
            var span = writer.GetSpan(length);
            var writtenBytes = Serialize(item, span);
            writer.Advance(writtenBytes);
        }
#endif

        public virtual void Serialize(in T item, IBufferWriter<byte> data) {
            var length = GetSpanLength(item);
            var span = data.GetSpan(length);
            var writtenBytes = Serialize(item, span);
            data.Advance(writtenBytes);
        }

        public abstract int GetSpanLength(in T item);

        public abstract int Serialize(in T item, Span<byte> data);

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
