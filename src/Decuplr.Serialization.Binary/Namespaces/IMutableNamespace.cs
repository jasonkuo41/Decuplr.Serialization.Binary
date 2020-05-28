using System;
using Decuplr.Serialization.Binary.Generic;

namespace Decuplr.Serialization.Binary.Namespaces {
    public interface IMutableNamespace {
        bool AddParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType>;
        bool AddGenericParserProvider<TParser>(Type genericType) where TParser : GenericParserProvider;
        bool AddSealedParser<T>(TypeParser<T> parser);
    }
}
