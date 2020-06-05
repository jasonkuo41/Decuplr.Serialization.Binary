using System.CodeDom.Compiler;
using System.ComponentModel;
using Decuplr.Serialization.Binary.Namespaces;


namespace Decuplr.Serialization.Binary.Internal {
    [GeneratedCode("Decuplr.Serialization.Binary.SourceGenerator", "1.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal partial class DefaultParserEntryPoint : AssemblyPackerEntryPoint {
        public override void LoadContext(INamespaceRoot root) {
            IDefaultParserNamespace defaultNamespace = root.DefaultNamespace;
        }
    }
}
