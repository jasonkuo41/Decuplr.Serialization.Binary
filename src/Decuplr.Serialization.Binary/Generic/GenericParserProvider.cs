using System;

namespace Decuplr.Serialization.Binary.Generic {
    public abstract class GenericParserProvider {
        public TypeParser<TGeneric> ProvideGenericParser<TGeneric, TNested>(IBinaryFormatter formatter, INamespaceProvider formatNamespace) {
            if (typeof(TNested).IsGenericType)
                throw new ArgumentException("Generic Parser doesn't implement Generic Nested Parser natively, try to cast to target Generic Nested Parser directly");
            return (TypeParser<TGeneric>)ProvideParser<TNested>(formatter, formatNamespace);
        }

        /*
        public bool TryProvideGenericParser<TGeneric, TNested>(IBinaryFormatter formatter, INamespaceProvider namespaceProvider, out TypeParser<TGeneric> parser) {
            parser = null;
            if (typeof(TNested).IsGenericType)
                return false;
            if (!TryProvideParser<TNested>(formatter, namespaceProvider, out var oParser))
                return false;
            parser = (TypeParser<TGeneric>)oParser;
            return true;
        }
        */

        protected abstract object ProvideParser<TNested>(IBinaryFormatter formatter, INamespaceProvider formatNamespace);
        //protected abstract bool TryProvideParser<TNested>(IBinaryFormatter formatter, INamespaceProvider formatNamespace, out object parser);
    }
}
