using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Decuplr.Serialization.Namespaces;

namespace Decuplr.Serialization.Binary {

    public interface IBinaryConvertionProvider<T> {
        BinaryConverter<T> GetConverter(IBinaryNamespaceDiscovery discovery);
        bool TryGetConverter(IBinaryNamespaceDiscovery discovery, [NotNullWhen(true)] out BinaryConverter<T>? converter);
    }

    public interface IBinaryNamespaceDiscovery : INamespaceDiscovery {
        BinaryConverter<T> GetConverter<T>();
        bool TryGetConverter<T>(out BinaryConverter<T> converter);
    }
}
