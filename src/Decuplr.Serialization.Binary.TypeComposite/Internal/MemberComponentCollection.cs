using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal {
    internal class MemberComponentCollection : IComponentCollection {

        private readonly List<ITypeSymbol> _symbols = new List<ITypeSymbol>();

        public IReadOnlyList<ITypeSymbol> Components => _symbols;

        public CompositeMethodNames AddComponent(ITypeSymbol symbol) {
            _symbols.Add(symbol);
            return CompositeMethodNames.AppendDefault($"Member_{symbol}");
        }
    }
}
