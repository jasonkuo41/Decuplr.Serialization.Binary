using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Decuplr.Sourceberg.Internal {
    internal struct SyntaxNodeAnalysisContextSource {

        public SyntaxNode Node { get; }

        public ISymbol? ContainingSymbol { get; }

        public SemanticModel SemanticModel { get; }

        public Compilation Compilation { get; }

        public ImmutableArray<AdditionalText> AdditionalTexts { get; }

        public AnalyzerConfigOptionsProvider AnalyzerConfigOptions { get; }

        public CancellationToken CancellationToken { get; }

        public SyntaxNodeAnalysisContextSource(SyntaxNode node,
                                           ISymbol? containingSymbol,
                                           SemanticModel semanticModel,
                                           Compilation compilation,
                                           ImmutableArray<AdditionalText> additionalTexts,
                                           AnalyzerConfigOptionsProvider analyzerConfigOptions,
                                           CancellationToken cancellationToken) {
            Node = node;
            ContainingSymbol = containingSymbol;
            SemanticModel = semanticModel;
            Compilation = compilation;
            AdditionalTexts = additionalTexts;
            AnalyzerConfigOptions = analyzerConfigOptions;
            CancellationToken = cancellationToken;
        }

        public SyntaxNodeAnalysisContextSource(SyntaxNode node,
                                           SemanticModel semanticModel,
                                           Compilation compilation,
                                           ImmutableArray<AdditionalText> additionalTexts,
                                           AnalyzerConfigOptionsProvider analyzerConfigOptions,
                                           CancellationToken cancellationToken) {
            Node = node;
            ContainingSymbol = semanticModel.GetDeclaredSymbol(node, cancellationToken) ?? semanticModel.GetSymbolInfo(node, cancellationToken).Symbol;
            SemanticModel = semanticModel;
            Compilation = compilation;
            AdditionalTexts = additionalTexts;
            AnalyzerConfigOptions = analyzerConfigOptions;
            CancellationToken = cancellationToken;
        }

        public static SyntaxNodeAnalysisContextSource FromContextSource(SyntaxNodeAnalysisContext syntaxContext) {
            return new SyntaxNodeAnalysisContextSource(syntaxContext.Node,
                                                       syntaxContext.ContainingSymbol,
                                                       syntaxContext.SemanticModel,
                                                       syntaxContext.Compilation!,
                                                       syntaxContext.Options.AdditionalFiles,
                                                       syntaxContext.Options.AnalyzerConfigOptionsProvider,
                                                       syntaxContext.CancellationToken);
        }

        public static SyntaxNodeAnalysisContextSource FromInherit(SyntaxNode nextSyntax, SyntaxNodeAnalysisContext syntaxContext) {
            return new SyntaxNodeAnalysisContextSource(nextSyntax,
                                                       syntaxContext.SemanticModel,
                                                       syntaxContext.Compilation!,
                                                       syntaxContext.Options.AdditionalFiles,
                                                       syntaxContext.Options.AnalyzerConfigOptionsProvider,
                                                       syntaxContext.CancellationToken);
        }

        public SyntaxNodeAnalysisContext<TSyntax>? ToActualContext<TSyntax>() where TSyntax : SyntaxNode {
            if (Node is null || !(Node is TSyntax syntaxNode))
                return null;
            return new SyntaxNodeAnalysisContext<TSyntax>(syntaxNode, ContainingSymbol, SemanticModel, AdditionalTexts, AnalyzerConfigOptions, CancellationToken);
        }
    }
}
