using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Generation {
    public interface ISymbolAnalyzerSetup {
        ISymbolAnalyzerSetupGroup<TSymbol> UseAnalyzer<TAnalyzer, TSymbol>() where TAnalyzer : SymbolActionAnalyzer<TSymbol> where TSymbol : ISymbol;
    }
}
