using System;

namespace Decuplr.Serialization.Binary.Namespaces {
    [Obsolete]
    public interface IParserNamespaceOwner : IMutableNamespace {
        LengthProvider? LengthProvider { get; set; }
        void ReplaceParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType>;
        void ReplaceParserProvider(Type genericType, Type genericParser);
        void ReplaceGenericParserProvider(Type genericParser, Type genericType);
        void ReplaceSealedParser<T>(TypeParser<T> parser);
    }
}
