using System;

namespace Decuplr.Serialization.Binary.Namespaces {
    public interface IMutableNamespace {
        bool AddParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType>;
        bool AddParserProvider(Type type, Type parserProvider);
        bool AddSealedParser<T>(TypeParser<T> parser);
    }
}
