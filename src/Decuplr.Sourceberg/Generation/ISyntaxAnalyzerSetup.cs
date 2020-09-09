using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Sourceberg.Generation {
    public interface ISyntaxAnalyzerSetup {
        ISyntaxAnalyzerSetupGroup<TSyntax> UseAnalyzer<TAnalyzer, TSyntax>(IEnumerable<SyntaxKind> selectedSyntaxKinds) where TAnalyzer : SyntaxNodeAnalyzer<TSyntax> where TSyntax : SyntaxNode;
    }
}
