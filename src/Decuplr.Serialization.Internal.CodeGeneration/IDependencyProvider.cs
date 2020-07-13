using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IComponentCollection {
        IReadOnlyList<ITypeSymbol> Components { get; }

        string AddComponent(ITypeSymbol symbol);
    }
}
