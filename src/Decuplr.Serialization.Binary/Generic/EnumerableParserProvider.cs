using System.Collections;
using System.Collections.Generic;

namespace Decuplr.Serialization.Binary {
    public class EnumerableParserProvider<TEnumerable> where TEnumerable : IEnumerable {

        public TypeParser<TEnumerable> ProviderParser<TContent>(IBinaryPacker formatter, INamespaceProvider formatNamespace, TypeParser<TContent>? genericParser) {

        }

    }
}
