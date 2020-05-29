using System;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {
    public abstract class GenericParserProvider {

        private class ParserProviderWrapper<T> : IParserProvider<T> {
            private readonly GenericParserProvider Provider;

            public ParserProviderWrapper(GenericParserProvider provider) {
                Provider = provider;
            }

            public TypeParser<T> ProvideParser(IParserDiscovery currentNamespace, INamespaceRoot rootNamespace) 
                => Provider.ProvideParser<T>(currentNamespace, rootNamespace);

            public bool TryProvideParser(IParserDiscovery currentNamespace, INamespaceRoot rootNamespace, out TypeParser<T> parser) 
                => Provider.TryProvideParser<T>(currentNamespace, rootNamespace, out parser);
        }

        public abstract TypeParser ProvideParser(IParserDiscovery formatter, INamespaceRoot formatNamespace);
        public abstract bool TryProvideParser(IParserDiscovery formatter, INamespaceRoot formatNamespace, out TypeParser parser);

        public virtual TypeParser<TGeneric> ProvideParser<TGeneric>(IParserDiscovery formatter, INamespaceRoot formatNamespace) {
            return (TypeParser<TGeneric>)ProvideParser(formatter, formatNamespace);
        }

        public virtual bool TryProvideParser<TGeneric>(IParserDiscovery formatter, INamespaceRoot formatNamespace, out TypeParser<TGeneric> parser) {
            var result = TryProvideParser(formatter, formatNamespace, out TypeParser generalParser);
            parser = (TypeParser<TGeneric>)generalParser;
            return result;
        }

        public virtual IParserProvider<T> CreateProvider<T>() => new ParserProviderWrapper<T>(this);
    }
}
