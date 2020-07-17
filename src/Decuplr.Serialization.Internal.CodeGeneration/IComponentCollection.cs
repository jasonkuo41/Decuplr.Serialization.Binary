using System.Collections.Generic;
using Decuplr.Serialization.CodeGeneration.ParserGroup;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IComponentCollection {
        IReadOnlyList<ITypeSymbol> Components { get; }

        ParserMethodGroup AddComponent(ITypeSymbol symbol);
    }
}
