using System;

namespace Decuplr.Serialization.Binary.Namespaces {
    public interface IMutableNamespace {
        bool AddParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType>;
        bool AddGenericParserProvider(Type parserProvider, Type genericType);
        bool AddSealedParser<T>(TypeParser<T> parser);
    }
}
