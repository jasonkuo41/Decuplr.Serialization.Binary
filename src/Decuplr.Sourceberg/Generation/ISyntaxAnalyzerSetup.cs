using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Generation {
    public interface ISyntaxAnalyzerSetup {
        ISyntaxAnalyzerSetupGroup<TSyntax> UseAnalyzer<TAnalyzer, TSyntax>() where TAnalyzer : SyntaxNodeAnalyzer<TSyntax> where TSyntax : SyntaxNode;
    }
}
