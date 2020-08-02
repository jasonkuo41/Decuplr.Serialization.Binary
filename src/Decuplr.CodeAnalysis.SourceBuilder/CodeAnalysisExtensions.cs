using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    static class CodeAnalysisExtensions {
        
        public static IEnumerable<INamedTypeSymbol> GetContainingTypes(this INamedTypeSymbol symbol) {
            var stack = new Stack<INamedTypeSymbol>();
            var currentSymbol = symbol;
            do {
                stack.Push(currentSymbol);
                currentSymbol = currentSymbol.ContainingType;
            } while (currentSymbol != null);
            return stack;
        }
    }
}
