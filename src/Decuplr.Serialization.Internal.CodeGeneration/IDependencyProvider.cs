using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IDependencyProvider {
        string GetComponentName(ITypeSymbol symbol);
    }
}
