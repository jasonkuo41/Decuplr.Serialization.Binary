using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.TypeComposite {
    public interface IComponentCollection {
        IReadOnlyList<ITypeSymbol> Components { get; }

        ComposerMethods AddComponent(ITypeSymbol symbol);
    }
}
