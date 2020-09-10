using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Decuplr.Sourceberg {
    public struct SyntaxNodeAnalysisContext<TSyntax> where TSyntax : SyntaxNode {
        
        /// <inheritdoc cref="SyntaxNodeAnalysisContext.Node"/>
        public TSyntax Node { get; }

        /// <inheritdoc cref="SyntaxNodeAnalysisContext.ContainingSymbol"/>
        public ISymbol? ContainingSymbol { get; }

        /// <inheritdoc cref="SyntaxNodeAnalysisContext.SemanticModel"/>
        public SemanticModel SemanticModel { get; }

        /// <inheritdoc cref="SyntaxNodeAnalysisContext.Compilation"/>
        public Compilation Compilation => SemanticModel.Compilation;

        /// <inheritdoc cref="AnalyzerOptions.AdditionalFiles"/>
        public ImmutableArray<AdditionalText> AdditionalTexts { get; }

        /// <inheritdoc cref="AnalyzerOptions.AnalyzerConfigOptionsProvider"/>
        public AnalyzerConfigOptionsProvider AnalyzerConfigOptions { get; }

        /// <inheritdoc cref="SyntaxNodeAnalysisContext.CancellationToken"/>
        public CancellationToken CancellationToken { get; }

        public SyntaxNodeAnalysisContext(TSyntax node,
                                         ISymbol? containingSymbol,
                                         SemanticModel semanticModel,
                                         ImmutableArray<AdditionalText> additionalTexts,
                                         AnalyzerConfigOptionsProvider analyzerConfigOptions,
                                         CancellationToken cancellationToken) {
            Node = node;
            ContainingSymbol = containingSymbol;
            SemanticModel = semanticModel;
            AdditionalTexts = additionalTexts;
            AnalyzerConfigOptions = analyzerConfigOptions;
            CancellationToken = cancellationToken;
        }

    }
}
