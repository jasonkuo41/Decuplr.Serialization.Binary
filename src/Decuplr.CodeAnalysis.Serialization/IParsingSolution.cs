using System.Collections;
using System.Threading;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IParsingSolution : IGroupValidationProvider {
        TypeParserInfo CreateParser(SchemaLayout targetLayout, IComponentProvider provider, string uniqueName);
    }
}
