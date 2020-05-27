using System;
using System.Collections.Generic;
using System.Diagnostics;
using Decuplr.Serialization.Binary.Internal.DefaultParsers;

namespace Decuplr.Serialization.Binary {
    internal class DefaultBinaryFormatter : BinaryPacker {

        private readonly Dictionary<string?, Dictionary<Type, object>> Serializers = new Dictionary<string?, Dictionary<Type, object>> {
            { string.Empty , new Dictionary<Type, object>() }
        };

        public DefaultBinaryFormatter(bool includeDefaultSerializers) {
            if (!includeDefaultSerializers)
                return;
            AddSealedParser(new Int32Parser());
        }

        public override void AddSealedParser<T>(TypeParser<T> parser) {
            Serializers[string.Empty].Add(typeof(T), parser);

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
    }
}
