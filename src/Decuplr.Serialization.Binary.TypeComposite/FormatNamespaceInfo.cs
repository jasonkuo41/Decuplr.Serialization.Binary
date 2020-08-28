using System.Collections.Generic;
using System.Linq;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public readonly struct FormatNamespaceInfo {

        private readonly IEnumerable<string> _withNamespaces;
        private readonly IEnumerable<string> _withPrioritizedNamespaces;

        public FormatNamespaceInfo(IEnumerable<string> withNamespaces, IEnumerable<string> withPrioritizedNamespaces) {
            _withNamespaces = withNamespaces;
            _withPrioritizedNamespaces = withPrioritizedNamespaces;
        }

        public IEnumerable<string> WithNamespaces => _withNamespaces ?? Enumerable.Empty<string>();
        public IEnumerable<string> WithPrioritizedNamespaces => _withPrioritizedNamespaces ?? Enumerable.Empty<string>();
    }
}
