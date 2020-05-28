using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Decuplr.Serialization.Binary {
    internal class ParserLocator : IParserNamespace {

        private readonly ParserNamespaces DiscoverNamespaces;
        public string[] CurrentNamespace { get; }

        public ParserLocator(ParserNamespaces namespaces) {
            DiscoverNamespaces = namespaces;
            CurrentNamespace = Array.Empty<string>();
        }

        public ParserLocator(ParserNamespaces namespaces, IEnumerable<string> targetNamespaces) : this(namespaces) {
            if (targetNamespaces is string[] strArray) {
                CurrentNamespace = strArray;
                return;
            }
            CurrentNamespace = targetNamespaces.ToArray();
        }

        private bool TryGetNonDefaultParser<T>(out TypeParser<T> parser) {
            for (var i = 0; i < CurrentNamespace.Length; ++i) {
                if (!DiscoverNamespaces.TryGetNamespace(CurrentNamespace[i], out var container))
                    continue;
                if (container.TryGetParser(this, out parser))
                    return true;
            }
            parser = null;
            return false;
        }

        public TypeParser<T> GetParser<T>() {
            // First we traverse all sub namespaces
            if (TryGetNonDefaultParser<T>(out var parser))
                return parser;
            // Finally we lookup to the default Namespace
            return DiscoverNamespaces.Default.GetParser<T>(this);
        }

        public bool TryGetParser<T>(out TypeParser<T> parser) {
            // First we traverse all sub namespaces
            if (TryGetNonDefaultParser(out parser))
                return true;
            // Finally we lookup to the default Namespace
            return DiscoverNamespaces.Default.TryGetParser(this, out parser);
        }
    }
}
