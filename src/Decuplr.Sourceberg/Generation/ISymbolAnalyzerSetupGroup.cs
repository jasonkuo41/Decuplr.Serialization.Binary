using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Generation {
    public interface ISymbolAnalyzerSetupGroup<TSymbol> where TSymbol : ISymbol {
        ISymbolAnalyzerSetupGroup<TSymbol> ThenUseAnalyzer<TAnalyzer>() where TAnalyzer : SymbolActionAnalyzer<TSymbol>;
        ISymbolAnalyzerSetupGroup<TNewSymbol> ThenUseAnalyzer<TAnalyzer, TNewSymbol>(Func<TSymbol, TNewSymbol> symbolFactory) where TNewSymbol : ISymbol;
        ISymbolAnalyzerSetupGroup<TNewSymbol> ThenUseAnalyzer<TAnalyzer, TNewSymbol>(Func<TSymbol, IEnumerable<TNewSymbol>> symbolFactory) where TNewSymbol : ISymbol;
    }
}
