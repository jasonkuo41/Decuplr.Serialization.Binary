using System;

namespace Decuplr.Serialization.Binary.Generic {
    public abstract class GenericParserProvider {
        
        public abstract TypeParser ProvideParser(IBinaryPacker formatter, INamespaceProvider formatNamespace);

        public virtual TypeParser<TGeneric> ProvideParser<TGeneric>(IBinaryPacker formatter, INamespaceProvider formatNamespace) {
            return (TypeParser<TGeneric>)ProvideParser(formatter, formatNamespace);
        }
    }
}
