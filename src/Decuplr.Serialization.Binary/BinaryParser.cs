using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    public abstract class BinaryParser<T> {
        public abstract void Deserialize(T value, Span<byte> destination);
        public abstract int GetBinaryLength(T value);

        public abstract SerializeResult TrySerialize(BinarySerializer serializer, ReadOnlySpan<byte> span, out T result);
        public abstract SerializeResult TryGetBinaryLength(BinarySerializer serializer, ReadOnlySpan<byte> span, out int length);
    }
}
