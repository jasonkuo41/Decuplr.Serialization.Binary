using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Decuplr.Sourceberg.Internal;
using Decuplr.Sourceberg.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Decuplr.Sourceberg {

    public abstract class SyntaxNodeAnalyzer<TSyntax> : SourceAnalyzerBase where TSyntax : SyntaxNode {

        public abstract void RunAnalysis(SyntaxNodeAnalysisContext<TSyntax> context, Action<CancellationToken> nextAction);

        internal override void InvokeAnalysis<TContext>(TContext context, Action<CancellationToken> nextAction) {
            if (!(context is SyntaxNodeAnalysisContextSource source))
                return;
            var analysisContext = source.ToActualContext<TSyntax>();
            if (analysisContext is null)
                return;
            RunAnalysis(analysisContext.Value, nextAction);
        }
    }

}
