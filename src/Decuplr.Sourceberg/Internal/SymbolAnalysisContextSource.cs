using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Decuplr.Sourceberg.Internal {
    internal struct SymbolAnalysisContextSource {
        public ISymbol Symbol { get; }

        public Compilation Compilation { get; }

        public ImmutableArray<AdditionalText> AdditionalFiles { get; }

        public AnalyzerConfigOptionsProvider AnalyzerConfigOptionsProvider { get; }

        public CancellationToken CancellationToken { get; }

        public SymbolAnalysisContextSource(ISymbol symbol,
                                           Compilation compilation,
                                           ImmutableArray<AdditionalText> additionalFiles,
                                           AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider,
                                           CancellationToken cancellationToken) {
            Symbol = symbol;
            Compilation = compilation;
            AdditionalFiles = additionalFiles;
            AnalyzerConfigOptionsProvider = analyzerConfigOptionsProvider;
            CancellationToken = cancellationToken;
        }

        public static SymbolAnalysisContextSource FromContextSource(ISymbol nextSymbol, SymbolAnalysisContext context)
            => new SymbolAnalysisContextSource(nextSymbol,
                                               context.Compilation,
                                               context.Options.AdditionalFiles,
                                               context.Options.AnalyzerConfigOptionsProvider,
                                               context.CancellationToken);

        public SymbolAnalysisContext<TSymbol>? ToActualContext<TSymbol>() where TSymbol : ISymbol {
            if (!(Symbol is TSymbol actualSymbol))
                return null;
            return new SymbolAnalysisContext<TSymbol>(actualSymbol, Compilation, AdditionalFiles, AnalyzerConfigOptionsProvider, CancellationToken);
        }

    }
}
