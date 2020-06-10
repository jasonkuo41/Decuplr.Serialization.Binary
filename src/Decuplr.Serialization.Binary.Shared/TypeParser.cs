using System;
using System.Buffers;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Schema;

namespace Decuplr.Serialization.Binary {
    public abstract class TypeParser<T> : TypeParser {

        public abstract bool TrySerialize(T value, Span<byte> destination, out int writtenBytes);

        public abstract int Serialize(T value, Span<byte> destination);

        public abstract int GetBinaryLength(T value);

        public abstract DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out T result);
    }

    /// <summary>
    /// Provides a base type for <see cref="TypeParser{T}"/>, do not inherit this directly
    /// </summary>
    public abstract class TypeParser {
        public virtual int? FixedSize { get; }
    }

	public abstract class TypeParserEx<T> : TypeParser {

        public virtual bool TrySerialize(T value, Span<byte> destination, out int writtenBytes) {
            unsafe {
                return TrySerialize(new ReadOnlySpan<T>(Unsafe.AsPointer(ref value), 1), destination, out writtenBytes);
            }
        }

        public virtual bool TrySerialize(T value, IBufferWriter<byte> writer) {
            unsafe {
                return TrySerialize(new ReadOnlySpan<T>(Unsafe.AsPointer(ref value), 1), writer);
            }
        }

        public virtual bool TrySerializeUnsafe(T value, Span<byte> destination, out int writtenBytes) {
            unsafe {
                return TrySerializeUnsafe(new ReadOnlySpan<T>(Unsafe.AsPointer(ref value), 1), destination, out writtenBytes);
            }
        }

        public virtual bool TrySerialize(ReadOnlySpan<T> value, Span<byte> destination, out int writtenBytes) {
            writtenBytes = -1;
            var length = GetLength(value);
            if (destination.Length < length)
                return false;
            TrySerializeUnsafe(value, destination, out writtenBytes);
            return true;
        }

        public abstract bool TrySerialize(ReadOnlySpan<T> value, IBufferWriter<byte> writter);

        public abstract bool TrySerializeUnsafe(ReadOnlySpan<T> value, Span<byte> destination, out int writtenBytes);

        public virtual bool Serialize(T value, Span<byte> destination, out int writtenBytes) {

        }

        public virtual int Serialize(ReadOnlySpan<T> value, Span<byte> destination) {
            var length = GetLength(value);
            if (destination.Length < length)
                throw new ArgumentOutOfRangeException(nameof(destination));
            return SerializeUnsafe(value, destination);
        }

        public abstract void Serialize(ReadOnlySpan<T> value, IBufferWriter<byte> writer);

        public abstract int SerializeUnsafe(ReadOnlySpan<T> value, Span<byte> destination);

		public abstract DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out T result);

        public abstract DeserializeResult TryDeserialize(in ReadOnlySequence<byte> sequence, out int readBytes, out T result);

		public abstract T Deserialize(ReadOnlySpan<byte> span, out int readBytes);

        public abstract T Deserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed);

		public abstract int GetLength(ReadOnlySpan<T> value);
	}

}
