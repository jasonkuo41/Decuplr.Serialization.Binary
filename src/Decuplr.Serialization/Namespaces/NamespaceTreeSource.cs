using System;
using System.Threading;

namespace Decuplr.Serialization.Namespaces {
    internal class NamespaceTreeSource : NamespaceNode, INamespaceTree {

        private readonly static ThreadLocal<Random> RevisionInitiate = new ThreadLocal<Random>(() => new Random(), false);

        public int Revision { get; private set; }

        public NamespaceTreeSource() : base ("default") {
            Revision = RevisionInitiate.Value.Next();
        }

        public NamespaceTreeSource(IReadOnlyNamespaceTree tree) : this() {
            // Create this empty instance then we clone the data to this instance
            CopyTo(tree, this);

            static void CopyTo(IReadOnlyNamespaceNode source, INamespaceNode node) {
                foreach (var item in source) {
                    node[item.Key.SourceAssembly, item.Key.Type] = item.Value;
                }
                foreach (var childNode in source.ChildNodes) {
                    CopyTo(childNode.Value, node.GetChildNamespace(childNode.Key));
                }
            }
        }

        public void Modified() => Revision++;

    }

}
