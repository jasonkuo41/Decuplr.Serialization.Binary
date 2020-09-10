using System;
using System.Threading;
using Decuplr.Sourceberg.Internal;
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
        public abstract void RunAnalysis(SymbolAnalysisContext<TSymbol> context, Action<CancellationToken> nextAction);

        internal override void InvokeAnalysis<TContext>(TContext context, Action<CancellationToken> nextAction) {
            if (!(context is SymbolAnalysisContextSource source))
                return;
            var analysisContext = source.ToActualContext<TSymbol>();
            if (analysisContext is null)
                return;
            RunAnalysis(analysisContext.Value, nextAction);
        }
    }
}
