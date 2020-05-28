using System.ComponentModel;

namespace Decuplr.Serialization.Binary.Internal {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class AssemblyPackerEntryPoint {
        public abstract void LoadContext(BinaryPacker packer);
    }
}
