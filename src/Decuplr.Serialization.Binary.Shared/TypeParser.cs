using System;
using System.Buffers;
using System.ComponentModel;

namespace Decuplr.Serialization.Binary {
    public abstract class TypeParser<T> : TypeParser {

        public virtual bool TrySerialize(in T value, Span<byte> destination, out int writtenBytes) {
            writtenBytes = -1;
            if (destination.Length < GetLength(value))
                return false;
            return TrySerializeUnsafe(value, destination, out writtenBytes);
        }

        public virtual bool TrySerialize(in T value, IBufferWriter<byte> writer) {
            var length = GetLength(value);
            var span = writer.GetSpan();
            // Should we silently fail this? This is kind of a huge problem that the IBufferWriter wasn't implement properly
            if (span.Length < length)
                return false;
            if (!TrySerializeUnsafe(value, span, out var writtenBytes))
                return false;
            writer.Advance(writtenBytes);
            return true;
        }

        public abstract bool TrySerializeUnsafe(in T value, Span<byte> destination, out int writtenBytes);

        public virtual int Serialize(in T value, Span<byte> destination) {
            if (destination.Length < GetLength(value))
                throw new ArgumentOutOfRangeException(nameof(destination));
            return SerializeUnsafe(value, destination);
        }

        public virtual void Serialize(in T value, IBufferWriter<byte> writer) {
            var length = GetLength(value);
            var span = writer.GetSpan();
            if (span.Length < length)
                throw new InvalidOperationException("IBufferWriter<byte> provided an insufficent length for the serialize object");
            writer.Advance(SerializeUnsafe(value, span));
        }

        public abstract int SerializeUnsafe(in T value, Span<byte> destination);

        public abstract DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out T result);

        public abstract DeserializeResult TryDeserialize(in ReadOnlySequence<byte> sequence, long offset, out long consumed, out T result);

        public abstract T Deserialize(ReadOnlySpan<byte> span, out int readBytes);

        public virtual T Deserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed) {

        }

        public abstract T Deserialize(ref MemoryNavigator<byte> state);

        public abstract int GetLength(in T value);

    }

    /// <summary>
    /// Provides a base type for <see cref="TypeParser{T}"/>, do not inherit this directly
    /// </summary>
    public abstract class TypeParser {
        public virtual int? FixedSize { get; }
    }
}
