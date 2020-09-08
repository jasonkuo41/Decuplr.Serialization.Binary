using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Decuplr.Sourceberg.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Sourceberg {

    internal interface IAnalyzerRegisterKind<TKind> {
        ImmutableArray<TKind> RegisteringKinds { get; }
    }

    public abstract class SyntaxNodeAnalyzer<TSyntax> : SourceAnalyzerBase, IAnalyzerRegisterKind<SyntaxKind> where TSyntax : SyntaxNode {

        public abstract ImmutableArray<SyntaxKind> RegisteringKinds { get; }

        public abstract void RunAnalysis(AnalysisContext<TSyntax> context, Action<CancellationToken> nextAction);

        internal override void InvokeAnalysis(AnalysisContextPrecusor contextPrecusor) {
            var source = contextPrecusor.Source as TSyntax;
            if (source is null)
                return;
            // Should we just skip this check since it may never be true?
            Debug.Assert(RegisteringKinds.Contains(source.Kind()));
            if (!RegisteringKinds.Contains(source.Kind()))
                return;
            var contextCollection = contextPrecusor.ContextProvider.GetContextCollection(source);
            var context = new AnalysisContext<TSyntax>(source, contextCollection, contextPrecusor.CancellationToken, contextPrecusor.OnDiagnostics, IsSupportedDiagnostic);
            RunAnalysis(context, contextPrecusor.NextAction);
        }
    }

}
