using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Generation {
    public interface ISyntaxAnalyzerSetupGroup<TSyntax> where TSyntax : SyntaxNode {
        ISyntaxAnalyzerSetupGroup<TSyntax> ThenUseAnalyzer<TAnalyzer>() where TAnalyzer : SyntaxNodeAnalyzer<TSyntax>;
        ISyntaxAnalyzerSetupGroup<TNewSyntax> ThenUseAnalyzer<TAnalyzer, TNewSyntax>(Func<TSyntax, TNewSyntax> syntaxFactory) where TAnalyzer : SyntaxNodeAnalyzer<TNewSyntax> where TNewSyntax : SyntaxNode;
        ISyntaxAnalyzerSetupGroup<TNewSyntax> ThenUseAnalyzer<TAnalyzer, TNewSyntax>(Func<TSyntax, IEnumerable<TNewSyntax>> syntaxFactory) where TAnalyzer : SyntaxNodeAnalyzer<TNewSyntax> where TNewSyntax : SyntaxNode;
    }
}
