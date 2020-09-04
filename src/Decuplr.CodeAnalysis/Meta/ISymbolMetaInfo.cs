using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Meta {
    [Obsolete]
    public interface ISymbolMetaInfo<out TSymbol> where TSymbol : ISymbol {
        internal ITypeSymbolProvider SymbolProvider { get; }

        TSymbol Symbol { get; }
    }
}
