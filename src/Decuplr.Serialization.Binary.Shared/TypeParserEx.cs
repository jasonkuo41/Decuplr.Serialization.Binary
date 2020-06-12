using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Decuplr.Serialization.Binary {
    public abstract class TypeParserEx<T> : TypeParser {

        public virtual bool TrySerialize(T value, Span<byte> destination, out int writtenBytes) {
            writtenBytes = -1;
            if (destination.Length < GetLength(value))
                return false;
            return TrySerializeUnsafe(value, destination, out writtenBytes);
        }

        public abstract bool TrySerialize(T value, IBufferWriter<byte> writer);
        public abstract bool TrySerializeUnsafe(T value, Span<byte> destination, out int writtenBytes);

        public virtual int Serialize(T value, Span<byte> destination) {
            if (destination.Length < GetLength(value))
                throw new ArgumentOutOfRangeException(nameof(destination));
            return SerializeUnsafe(value, destination);
        }

        public abstract void Serialize(T value, IBufferWriter<byte> writer);

        public abstract int SerializeUnsafe(T value, Span<byte> destination);

        public abstract DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out T result);

        public abstract DeserializeResult TryDeserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed, out T result);

        public abstract T Deserialize(ReadOnlySpan<byte> span, out int readBytes);

        public abstract T Deserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed);

        public abstract int GetLength(T value);
    }

}
