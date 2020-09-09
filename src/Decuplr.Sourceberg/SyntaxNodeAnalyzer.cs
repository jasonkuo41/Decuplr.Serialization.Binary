using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Decuplr.Sourceberg.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Decuplr.Sourceberg {

    public abstract class SyntaxNodeAnalyzer<TSyntax> : SourceAnalyzerBase where TSyntax : SyntaxNode {

        public abstract void RunAnalysis(SyntaxNodeAnalysisContext<TSyntax> context, Action<CancellationToken> nextAction);

        internal override void InvokeAnalysis<TContext>(TContext context, Action<CancellationToken> nextAction) {
            if (!(context is SyntaxNodeAnalysisContext<TSyntax> actualContext))
                return;
            var contextCollection = contextPrecusor.ContextProvider.GetContextCollection(source);
            var acontext = new AnalysisContext<TSyntax>(source, contextCollection, contextPrecusor.CancellationToken, contextPrecusor.OnDiagnostics, IsSupportedDiagnostic);
            RunAnalysis(acontext, contextPrecusor.NextAction);
        }
    }

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
    }
}
