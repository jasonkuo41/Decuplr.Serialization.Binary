using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Generation {
    public interface ISymbolAnalyzerSetup {
        ISymbolAnalyzerSetupGroup<TSymbol> UseAnalyzer<TAnalyzer, TSymbol>(IEnumerable<SymbolKind> selectedSymbolKind) where TAnalyzer : SymbolActionAnalyzer<TSymbol> where TSymbol : ISymbol;
    }
}
