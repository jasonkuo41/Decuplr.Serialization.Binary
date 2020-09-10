using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Decuplr.Sourceberg.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Sourceberg.Internal {

    internal class SyntaxAnalyzerSetup : ISyntaxAnalyzerSetup {

        private readonly List<AnalyzerGroupInfo<SyntaxNode, SyntaxNodeAnalysisContextSource, SyntaxKind>> _anaylzerGroups = new List<AnalyzerGroupInfo<SyntaxNode, SyntaxNodeAnalysisContextSource, SyntaxKind>>();

        public IReadOnlyList<AnalyzerGroupInfo<SyntaxNode, SyntaxNodeAnalysisContextSource, SyntaxKind>> AnalyzerGroups => _anaylzerGroups;

        public ISyntaxAnalyzerSetupGroup<TSyntax> UseAnalyzer<TAnalyzer, TSyntax>(IEnumerable<SyntaxKind> selectedSyntaxKinds)
            where TAnalyzer : SyntaxNodeAnalyzer<TSyntax>
            where TSyntax : SyntaxNode {
            var group = new AnalyzerGroup<SyntaxNode, SyntaxNodeAnalysisContextSource>();
            var first = new SyntaxAnalyzerGroupNode<TAnalyzer, TSyntax>(group);
            _anaylzerGroups.Add((group, selectedSyntaxKinds.ToImmutableArray()));
            return first;
        }
    }

}
