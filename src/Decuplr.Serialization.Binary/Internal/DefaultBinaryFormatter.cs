using System;
using System.Collections.Generic;
using System.Diagnostics;
using Decuplr.Serialization.Binary.Internal.DefaultParsers;

namespace Decuplr.Serialization.Binary {
    internal class DefaultBinaryFormatter : BinaryFormatter {

        private readonly Dictionary<string?, Dictionary<Type, object>> Serializers = new Dictionary<string?, Dictionary<Type, object>> {
            { string.Empty , new Dictionary<Type, object>() }
        };

        public DefaultBinaryFormatter(bool includeDefaultSerializers) {
            if (!includeDefaultSerializers)
                return;
            AddImmutableParser(new Int32Parser());
        }

        public override void AddImmutableParser<T>(BinaryParser<T> parser) {
            Serializers[string.Empty].Add(typeof(T), parser);

        }

        public override bool TryGetFormatter<T>(out BinaryParser<T> parser) {
            var found = Serializers[string.Empty].TryGetValue(typeof(T), out var result);

            Debug.Assert(result is BinaryParser<T>);

            parser = (BinaryParser<T>)result;
            return found;
        }

        public override IBinaryFormatter GetNamespace(string parserNamespace) {
            throw new NotImplementedException();
        }

        public override void AddParserProvider<T>(Func<IBinaryFormatter, IBinaryNamespace, BinaryParser<T>> parserSource) {
            throw new NotImplementedException();
        }
    }
}
