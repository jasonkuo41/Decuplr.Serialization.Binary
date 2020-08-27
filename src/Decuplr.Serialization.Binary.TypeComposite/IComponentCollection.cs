using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public interface IComponentCollection {
        IReadOnlyList<ITypeSymbol> Components { get; }

        CompositeMethodNames AddComponent(ITypeSymbol symbol);
    }
}
