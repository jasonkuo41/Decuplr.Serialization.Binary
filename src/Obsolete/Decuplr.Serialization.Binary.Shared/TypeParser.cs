using System;
using System.Buffers;
using System.ComponentModel;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Provides the base class for serializing and deserializing objects into it's binary counterpart
    /// </summary>
    /// <typeparam name="T">The target type for serialization and deserialization</typeparam>
    public abstract class TypeParser<T> : TypeParser {

        // Should we provide a ITypeParser<T> for people who would like to create a serializer and deserializer manually?
        // And then we can wrap around the interface using a wrapper so that it can be added to parser pool
        // Though, I see no point of doing so, unless dealing with complex types that our scematic designing tool might not resolve it well
        // The performance would definetly be worse then the source generated ones

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

        public virtual DeserializeResult TryDeserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed, out T result) {
            // Create cursor for this sequence
            var cursor = new SequenceCursor<byte>(in sequence);
            var deserailizedResult = TryDeserialize(ref cursor, out result);
            consumed = cursor.Position;
            return deserailizedResult;
        }

        public abstract DeserializeResult TryDeserialize(ref SequenceCursor<byte> cursor, out T result);

        public abstract T Deserialize(ReadOnlySpan<byte> span, out int readBytes);

        public virtual T Deserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed) {
            // Create cursor for this sequence
            var cursor = new SequenceCursor<byte>(in sequence);
            var result = Deserialize(ref cursor);
            consumed = cursor.Position;
            return result;
        }

        public abstract T Deserialize(ref SequenceCursor<byte> cursor);

        public abstract int GetLength(in T value);

    }

    /// <summary>
    /// Provides a base, non-generic type for <see cref="TypeParser{T}"/>
    /// </summary>
    /// <remarks>
    /// Do not inherit this class directly
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class TypeParser {
        public virtual int? FixedSize { get; }
    }
}
