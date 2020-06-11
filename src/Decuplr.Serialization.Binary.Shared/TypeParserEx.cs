using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Decuplr.Serialization.Binary {
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

        public virtual int Serialize(T value, Span<byte> destination) {
            unsafe {
                return Serialize(new ReadOnlySpan<T>(Unsafe.AsPointer(ref value), 1), destination);
            }
        }

        public virtual int Serialize(ReadOnlySpan<T> value, Span<byte> destination) {
            var length = GetLength(value);
            if (destination.Length < length)
                throw new ArgumentOutOfRangeException(nameof(destination));
            return SerializeUnsafe(value, destination);
        }

        public abstract void Serialize(ReadOnlySpan<T> value, IBufferWriter<byte> writer);

        public abstract int SerializeUnsafe(ReadOnlySpan<T> value, Span<byte> destination);

        public abstract int GetItemCount(ReadOnlySpan<byte> span);

        public virtual DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out T result) {
            unsafe {
                result = default;
                return TryDeserialize(span, new Span<T>(Unsafe.AsPointer(ref result), 1), out readBytes);
            }
        }

        public abstract DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, Span<T> destination, out int readBytes);

        public abstract DeserializeResult TryDeserialize(in ReadOnlySequence<byte> sequence, Span<T> destination, out SequencePosition consumed);

        public virtual DeserializeResult TryDeserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed, out T result) {
            unsafe {
                result = default;
                return TryDeserialize(in sequence, new Span<T>(Unsafe.AsPointer(ref result), 1), out consumed);
            }
        }

        public abstract int Deserialize(ReadOnlySpan<byte> span, Span<T> destination);

        public virtual T Deserialize(ReadOnlySpan<byte> span, out int readBytes) {
            unsafe {
                T result = default;
                readBytes = Deserialize(span, new Span<T>(Unsafe.AsPointer(ref result), 1));
                return result;
            }
        }

        public abstract SequencePosition Deserialize(in ReadOnlySequence<byte> sequence, Span<T> destination);

        public virtual T Deserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed) {
            unsafe {
                T result = default;
                consumed = Deserialize(in sequence, new Span<T>(Unsafe.AsPointer(ref result), 1));
                return result;
            }
        }

        public abstract int GetLength(ReadOnlySpan<T> value);
    }

}
