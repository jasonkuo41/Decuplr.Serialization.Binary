using System;

namespace Decuplr.Serialization.Binary.Namespaces {
    public interface IParserNamespaceOwner : IMutableNamespace {
        LengthProvider? LengthProvider { get; set; }
        void ReplaceParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType>;
        void ReplaceGenericParserProvider<TParser>(Type genericType) where TParser : GenericParserProvider;
        void ReplaceSealedParser<T>(TypeParser<T> parser);
    }
}
