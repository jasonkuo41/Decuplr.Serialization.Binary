using System;
using System.Buffers;
using System.Collections.Generic;
using System.Security.Cryptography;
using Decuplr.Serialization.Binary.Internal;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// This parser is responsible for generic types that implements <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The containing element</typeparam>
    [MutatableParser]
    public class CollectionParser<TCollection, TType> : GenericParser<TType> where TCollection : IEnumerable<TType> {

        protected readonly IBinaryFormatter Formatter;

        protected CollectionParser(IBinaryFormatter formatter) {
            Formatter = formatter;
        }

        public abstract int GetBinaryLength(ReadOnlySpan<T> value);
        public abstract int GetBinaryLength(IEnumerable<T> value);

        public abstract bool TrySerialize(ReadOnlySpan<T> value, Span<byte> destination, out int writtenBytes);
        public abstract bool TrySerialize(IEnumerable<T> value, Span<byte> destination, out int writtenBytes);

        public abstract DeserializeResult TryGetCollectionLength(ReadOnlySpan<byte> span);

        public DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, Span<T> destination, out int readBytes);
        public DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, ArrayPool<T> pool, out int readBytes, out T[] result, out int writtenLength);
        public DeserializeResult TryDeserialize<TCollection>(ReadOnlySpan<byte> span, out int readBytes, out TCollection result) where TCollection : IEnumerable<T>;
    }

    /// <summary>
    /// This parser is dedicated for <see cref="Span{T}"/>, <see cref="ReadOnlySpan{T}"/>, <see cref="Memory{T}"/> and <see cref="ReadOnlyMemory{T}"/>
    /// </summary>
    public abstract class SpanParser<T> : GenericParser<T> {

    }

    /// <summary>
    /// This parser is responsible for packing up collections of uncertain length
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DynamicMemoryParser<T> {

    }

    /// <summary>
    /// This parses the generic type of a certain other type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericParser<T> {
        public abstract int 
    }
}
