using System.Collections.Generic;
using Decuplr.Serialization.CodeGeneration.TypeComposers;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IComponentCollection {
        IReadOnlyList<ITypeSymbol> Components { get; }

        ComposerMethods AddComponent(ITypeSymbol symbol);
    }
}
