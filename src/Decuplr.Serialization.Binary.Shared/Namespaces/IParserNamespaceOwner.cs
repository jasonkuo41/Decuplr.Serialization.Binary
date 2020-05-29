using System;

namespace Decuplr.Serialization.Binary.Namespaces {
    public interface IParserNamespaceOwner : IMutableNamespace {
        void ReplaceParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType>;
        void ReplaceGenericParserProvider<TParser>(Type genericType) where TParser : GenericParserProvider;
        void ReplaceSealedParser<T>(TypeParser<T> parser);
    }
}
