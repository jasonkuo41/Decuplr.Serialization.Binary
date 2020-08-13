using System.ComponentModel;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary.Internal {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class AssemblyPackerEntryPoint {
        public abstract void LoadContext(INamespaceRoot packer);
    }
}
