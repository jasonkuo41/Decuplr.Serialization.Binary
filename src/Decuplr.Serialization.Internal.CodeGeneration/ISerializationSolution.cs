using System.Threading;
using Decuplr.Serialization.CodeGeneration.TypeComposite;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface ISerializationSolution {
        GeneratedParserInfo Generate(IComponentProvider provider, SchemaLayout layout, INamedTypeSymbol targetSymbol, CancellationToken ct);
    }
}
