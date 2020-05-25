using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    public abstract class BinaryParser<T> {

        public virtual int? FixedSize { get; }

        public abstract bool TrySerialize(T value, Span<byte> destination, out int writtenBytes);
        public abstract int GetBinaryLength(T value);

        public abstract SerializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out T result);
    }
}
