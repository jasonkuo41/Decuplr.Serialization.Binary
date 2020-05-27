using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Decuplr.Serialization.Binary.Generic {
    public abstract class GenericParserProvider<T0> : GenericParserProvider {

        private static readonly IReadOnlyList<bool> IsGenericTypes = new bool[] { typeof(T0).IsGenericType };

        public override TypeParser ProvideParser(IBinaryPacker formatter, INamespaceProvider formatNamespace) {

            TypeParser<T0>? parser = null;

            if (IsGenericTypes[0]) {
                if (!formatter.TryGetGenericParserProvider(typeof(T0), out var genericParserProvider))
                    throw new NotSupportedException($"Unable to locate generic parser for {typeof(T0)} in {typeof(GenericParserProvider<T0>)}");
                parser = (TypeParser<T0>)genericParserProvider.ProvideParser(formatter, formatNamespace);
            }
            return ProvideParserInternal(formatter, formatNamespace, parser);
        }

        protected abstract TypeParser ProvideParserInternal(IBinaryPacker formatter, INamespaceProvider formatNamespace, TypeParser<T0>? firstParser);
    }

    // TODO, Generate more T1, T2, T3 types
}
