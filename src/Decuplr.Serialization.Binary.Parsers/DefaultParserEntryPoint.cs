using System.Runtime.CompilerServices;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.Binary.Annotations.Internal;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;

[assembly: DefaultParserAssembly(typeof(DefaultParserEntryPoint))]
[assembly: InternalsVisibleTo("Decuplr.Serialization.Binary")]

namespace Decuplr.Serialization.Binary {

    // This class will be automatically generated
    internal partial class DefaultParserEntryPoint : AssemblyPackerEntryPoint {
        public override void LoadContext(INamespaceRoot packer) {
            throw new System.NotImplementedException();
        }
    }

}
