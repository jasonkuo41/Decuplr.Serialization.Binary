using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
    /// <summary>
    /// Represents a <see cref="ISymbol"/> Action based Analyzers.
    /// </summary>
    /// <typeparam name="TSymbol">The kind of symbol to be analyzed</typeparam>
    public abstract class SymbolActionAnalyzer<TSymbol> : SourceAnalyzerBase where TSymbol : ISymbol {
        /// <summary>
        /// Perform analysis on the symbol and provide additional information to the symbol
        /// </summary>
        /// <param name="currentSymbol"></param>
        public abstract void RunAnalysis(AnalysisContext<TSymbol> currentSymbol);
    }
}
