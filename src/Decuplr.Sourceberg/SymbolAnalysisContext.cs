using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Decuplr.Sourceberg {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSymbol"></typeparam>
    public struct SymbolAnalysisContext<TSymbol> where TSymbol : ISymbol {

        public TSymbol Symbol { get; }

        public Compilation Compilation { get; }

        public ImmutableArray<AdditionalText> AdditionalFiles { get; }

        public AnalyzerConfigOptionsProvider AnalyzerConfigOptionsProvider { get; }

        public CancellationToken CancellationToken { get; }

        public SymbolAnalysisContext(TSymbol symbol,
                                     Compilation compilation,
                                     ImmutableArray<AdditionalText> additionalFiles,
                                     AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider,
                                     CancellationToken cancellationToken) {
            Symbol = symbol;
            Compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
            AdditionalFiles = additionalFiles;
            AnalyzerConfigOptionsProvider = analyzerConfigOptionsProvider ?? throw new ArgumentNullException(nameof(analyzerConfigOptionsProvider));
            CancellationToken = cancellationToken;
        }
    }
}