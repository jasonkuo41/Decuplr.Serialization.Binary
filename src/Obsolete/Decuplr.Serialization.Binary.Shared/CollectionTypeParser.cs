using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Decuplr.Serialization.Binary.Internal;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// This parser is responsible for generic types that implements <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The containing element</typeparam>
    public abstract class CollectionTypeParser<TCollection, TType> : TypeParser<TCollection> where TCollection : IEnumerable<TType> {
        public sealed override int? FixedSize => null;
        public abstract bool SupportsPooling { get; }

        public abstract DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, int collectionLength, out int readBytes, out TType result);
        public abstract DeserializeResult TryDerserailize(ReadOnlySpan<byte> span, ArrayPool<TType> pool, out int readBytes, out TCollection result, out TType[] backingArray);
    }

}
