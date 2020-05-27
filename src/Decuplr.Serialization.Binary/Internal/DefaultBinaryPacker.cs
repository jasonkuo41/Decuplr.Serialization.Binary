using System;
using System.Collections.Generic;
using System.Diagnostics;
using Decuplr.Serialization.Binary.Internal.DefaultParsers;

namespace Decuplr.Serialization.Binary {
    internal partial class DefaultBinaryPacker : BinaryPacker {

        private class ParserNamespaces {
            public static readonly IReadOnlyList<string> DefaultNamespaceTitle { get; } = new string[] { string.Empty, "default", "Default", "DEFAULT" };

            private readonly Dictionary<Type, ParserContainer> Namespaces = new Dictionary<Type, ParserContainer>();

            public ParserContainer Default => 

            public ParserContainer GetNamespace(string @namespace) {

            }

            public ParserContainer this [string @namespace] => GetNamespace(@namespace);
        }

        private class ParserContainer : IDictionary<Type, TypeParser> {

        }

        private class LiteBinaryPacker : IBinaryPacker {
            public string CurrentNamespace { get; }

            public bool TryGetParser<T>(out TypeParser<T> parser) {
                throw new NotImplementedException();
            }
        }

        private readonly ParserNamespaces Namespaces = new ParserNamespaces();

        public DefaultBinaryPacker(bool includeDefaultSerializers) {
            if (!includeDefaultSerializers)
                return;
            LoadDefaultParser(Namespaces);
        }

        public override void AddSealedParser<T>(TypeParser<T> parser) {
            Namespaces.Default.Add(typeof(T), parser);

        }

        public override bool TryGetParser<T>(out TypeParser<T> parser) {
            var found = Serializers[string.Empty].TryGetValue(typeof(T), out var result);

            Debug.Assert(result is TypeParser<T>);

            parser = (TypeParser<T>)result;
            return found;
        }

        public override IBinaryPacker GetNamespace(string parserNamespace) {
            throw new NotImplementedException();
        }

        public override void AddParserProvider<T>(Func<IBinaryPacker, INamespaceProvider, TypeParser<T>> parserSource) {
            throw new NotImplementedException();
        }

        private static partial void LoadDefaultParser(ParserNamespaces namespaces);
    }
}
