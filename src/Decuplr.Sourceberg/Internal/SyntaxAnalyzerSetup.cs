using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Decuplr.Sourceberg.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Sourceberg.Internal {

    internal class SyntaxAnalyzerSetup : ISyntaxAnalyzerSetup {

        private readonly List<AnalyzerGroupInfo<SyntaxNode, SyntaxKind>> _anaylzerGroups = new List<AnalyzerGroupInfo<SyntaxNode, SyntaxKind>>();

        public IReadOnlyList<AnalyzerGroupInfo<SyntaxNode, SyntaxKind>> AnalyzerGroups => _anaylzerGroups;

        public ISyntaxAnalyzerSetupGroup<TSyntax> UseAnalyzer<TAnalyzer, TSyntax>(IEnumerable<SyntaxKind> selectedSyntaxKinds)
            where TAnalyzer : SyntaxNodeAnalyzer<TSyntax>
            where TSyntax : SyntaxNode {
            var group = new AnalyzerGroup<SyntaxNode>();
            var first = new SyntaxAnalyzerGroupNode<TAnalyzer, TSyntax>(group);
            _anaylzerGroups.Add((group, selectedSyntaxKinds.ToImmutableArray()));
            return first;
        }
    }

}
