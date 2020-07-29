using System.Threading;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface ISerializationSolution {
        GeneratedParserInfo Generate(IComponentProvider provider, SchemaLayout layout, INamedTypeSymbol targetSymbol, CancellationToken ct);
    }
}
