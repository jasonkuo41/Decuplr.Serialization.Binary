using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis {
    public static class SymbolExtensions {
        public static bool SourceEquals(this ISymbol symbol, ISymbol target, SymbolEqualityComparer comparer) => symbol.OriginalDefinition.Equals(target.OriginalDefinition, comparer);
        public static bool IsSourceDefinition(this ISymbol symbol) => symbol.OriginalDefinition.Equals(symbol, SymbolEqualityComparer.Default);
    }
}
