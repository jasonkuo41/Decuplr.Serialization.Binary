using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {
    internal class ParserNamespaces {
        public static IReadOnlyList<string> DefaultNamespaceTitle { get; } = new string[] { string.Empty, "default", "Default", "DEFAULT" };

        private readonly INamespaceRoot NamespaceRoot;
        private readonly ConcurrentDictionary<string, ParserContainer> Namespaces = new ConcurrentDictionary<string, ParserContainer>();

        public DefaultNamespaceParserContainer Default { get; }

        public ParserNamespaces(BinaryPacker packer) {
            NamespaceRoot = packer;

            Default = new DefaultNamespaceParserContainer(packer);
            // This makes all namespace with default namespace title to map
            Namespaces = new ConcurrentDictionary<string, ParserContainer>(DefaultNamespaceTitle.Select(x => new KeyValuePair<string, ParserContainer>(x, Default)));
            // Optimizations Point : For all `Internal` namespaces, we should also be able to really fetch them real fast without string lookups
            // Note : This might only speed up startup times for most format, but it'll be faster for us to resolve a anonymous object
        }

        public ParserContainer this[string @namespace] => GetNamespace(@namespace);

        public ParserContainer GetNamespace(string @namespace) => Namespaces[@namespace];

        public ParserContainer GetOrAddNamespace(string @namespace) => Namespaces.GetOrAdd(@namespace, key => new ParserContainer(key, NamespaceRoot));

        public bool TryGetNamespace(string @namespace, out ParserContainer container) => Namespaces.TryGetValue(@namespace, out container);

        public bool TryAddNamespace(string @namespace, out ParserContainer container) {
            var createdContainer = new ParserContainer(@namespace, NamespaceRoot);
            if (Namespaces.TryAdd(@namespace, createdContainer)) {
                container = createdContainer;
                return true;
            }
            container = null;
            return false;
        }

    }
}
