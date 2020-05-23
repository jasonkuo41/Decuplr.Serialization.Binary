using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    public interface IBinaryParser<T> {
        bool TrySerialize(ReadOnlySpan<byte> span, out T result);
        T Serialize(ReadOnlySpan<byte> span);
        void Deserialize(T value, Span<byte> destination);
        int GetBinaryLength(T value);
    }
}
