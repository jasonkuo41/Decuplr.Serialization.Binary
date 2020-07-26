using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    public interface IComponentCollection {
        IReadOnlyList<ITypeSymbol> Components { get; }

        ComposerMethods AddComponent(ITypeSymbol symbol);
    }
}
