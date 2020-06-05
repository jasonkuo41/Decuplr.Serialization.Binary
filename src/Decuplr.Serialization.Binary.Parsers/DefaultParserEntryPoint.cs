using System.Runtime.CompilerServices;
using Decuplr.Serialization.Binary.Annotations.Internal;
using Decuplr.Serialization.Binary.Internal;

[assembly: DefaultParserAssembly(typeof(DefaultParserEntryPoint))]
[assembly: InternalsVisibleTo("Decuplr.Serialization.Binary")]

namespace Decuplr.Serialization.Binary.Internal {

    // This class will be automatically generated and inherit AssemblyPackerEntryPoint
    internal partial class DefaultParserEntryPoint { }

}
