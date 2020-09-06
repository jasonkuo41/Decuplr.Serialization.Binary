using System.Threading;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
    public abstract class SyntaxNodeAnalyzer<TSyntax> : SourceAnalyzerBase where TSyntax : SyntaxNode {
        public abstract void RunAnalysis(AnalysisContext<TSyntax> context, CancellationToken ct);
    }

}
