using System;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary.Generic {
    public abstract class GenericParserProvider {
        
        public abstract TypeParser ProvideParser(IParserNamespace formatter, INamespaceRoot formatNamespace);
        public abstract bool TryProvideParser(IParserNamespace formatter, INamespaceRoot formatNamespace, out TypeParser parser);

        public virtual TypeParser<TGeneric> ProvideParser<TGeneric>(IParserNamespace formatter, INamespaceRoot formatNamespace) {
            return (TypeParser<TGeneric>)ProvideParser(formatter, formatNamespace);
        }

        public virtual bool TryProvideParser<TGeneric>(IParserNamespace formatter, INamespaceRoot formatNamespace, out TypeParser<TGeneric> parser) {
            var result = TryProvideParser(formatter, formatNamespace, out TypeParser generalParser);
            parser = (TypeParser<TGeneric>)generalParser;
            return result;
        }
    }
}
