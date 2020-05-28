using System;
using Decuplr.Serialization.Binary.Generic;

namespace Decuplr.Serialization.Binary.Namespaces {
    public interface IParserNamespaceSource : IMutableNamespace {
        void EnforceParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType>;
        void EnforceGenericParserProvider<TParser>(Type genericType) where TParser : GenericParserProvider;
        void EnforceSealedParser<T>(TypeParser<T> parser);
    }
}
