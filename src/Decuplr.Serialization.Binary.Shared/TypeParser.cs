using System;
using System.Buffers;

namespace Decuplr.Serialization.Binary {
    public abstract class TypeParser<T> : TypeParser {

        public virtual bool TrySerialize(in T value, Span<byte> destination, out int writtenBytes) {
            writtenBytes = -1;
            if (destination.Length < GetLength(value))
                return false;
            return TrySerializeUnsafe(value, destination, out writtenBytes);
        }

        public abstract bool TrySerialize(in T value, IBufferWriter<byte> writer);
        public abstract bool TrySerializeUnsafe(in T value, Span<byte> destination, out int writtenBytes);

        public virtual int Serialize(in T value, Span<byte> destination) {
            if (destination.Length < GetLength(value))
                throw new ArgumentOutOfRangeException(nameof(destination));
            return SerializeUnsafe(value, destination);
        }

        public abstract void Serialize(in T value, IBufferWriter<byte> writer);

        public abstract int SerializeUnsafe(in T value, Span<byte> destination);

        public abstract DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out T result);

        public abstract DeserializeResult TryDeserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed, out T result);

        public abstract T Deserialize(ReadOnlySpan<byte> span, out int readBytes);

        public abstract T Deserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed);

        public abstract int GetLength(in T value);

    }

    /// <summary>
    /// Provides a base type for <see cref="TypeParser{T}"/>, do not inherit this directly
    /// </summary>
    public abstract class TypeParser {
        public virtual int? FixedSize { get; }
    }

}
