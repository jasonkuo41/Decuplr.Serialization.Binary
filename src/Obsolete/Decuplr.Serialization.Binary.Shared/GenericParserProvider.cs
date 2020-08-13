using System;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {
    public abstract class GenericParserProvider {

        private class ParserProviderWrapper<T> : IParserProvider<T> {
            private readonly GenericParserProvider Provider;

            public ParserProviderWrapper(GenericParserProvider provider) {
                Provider = provider;
            }

            public TypeParser<T> ProvideParser(IParserDiscovery currentNamespace) 
                => Provider.ProvideParser<T>(currentNamespace);

            public bool TryProvideParser(IParserDiscovery currentNamespace, out TypeParser<T> parser) 
                => Provider.TryProvideParser<T>(currentNamespace, out parser);
        }

        public abstract TypeParser ProvideParser(IParserDiscovery discovery);
        public abstract bool TryProvideParser(IParserDiscovery discovery, out TypeParser parser);

        public virtual TypeParser<TGeneric> ProvideParser<TGeneric>(IParserDiscovery formatter) {
            return (TypeParser<TGeneric>)ProvideParser(formatter);
        }

        public virtual bool TryProvideParser<TGeneric>(IParserDiscovery discovery, out TypeParser<TGeneric> parser) {
            var result = TryProvideParser(discovery, out TypeParser generalParser);
            parser = (TypeParser<TGeneric>)generalParser;
            return result;
        }

        public virtual IParserProvider<T> CreateProvider<T>() => new ParserProviderWrapper<T>(this);
    }
}
