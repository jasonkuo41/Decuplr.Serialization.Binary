using System;
using System.Collections.Generic;
using Decuplr.Sourceberg.Generation;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Internal {
    internal class SyntaxAnalyzerGroupNode<TSourceAnalyzer, TSyntax> : AnalyzerGroupNode<SyntaxNode>, ISyntaxAnalyzerSetupGroup<TSyntax>
        where TSourceAnalyzer : SyntaxNodeAnalyzer<TSyntax>
        where TSyntax : SyntaxNode {

        public override Type AnalyzerType { get; } = typeof(TSourceAnalyzer);

        public SyntaxAnalyzerGroupNode(AnalyzerGroup<SyntaxNode> group)
            : base(group) {
        }

        private SyntaxAnalyzerGroupNode(AnalyzerGroupNode<SyntaxNode> node)
            : base(node.Group) {
        }

        private SyntaxAnalyzerGroupNode(AnalyzerGroupNode<SyntaxNode> node, Func<SyntaxNode, SyntaxNode> syntaxFactory)
            : base(node.Group, syntaxFactory) {
        }

        private SyntaxAnalyzerGroupNode(AnalyzerGroupNode<SyntaxNode> node, Func<SyntaxNode, IEnumerable<SyntaxNode>>? transitionMultiple)
            : base(node.Group, transitionMultiple) {
        }

        public ISyntaxAnalyzerSetupGroup<TSyntax> ThenUseAnalyzer<TAnalyzer>() where TAnalyzer : SyntaxNodeAnalyzer<TSyntax> {
            var nextItem = new SyntaxAnalyzerGroupNode<TAnalyzer, TSyntax>(this);
            SetNextNode(nextItem);
            return nextItem;
        }

        public ISyntaxAnalyzerSetupGroup<TNewSyntax> ThenUseAnalyzer<TAnalyzer, TNewSyntax>(Func<TSyntax, TNewSyntax> syntaxFactory) where TAnalyzer : SyntaxNodeAnalyzer<TNewSyntax> where TNewSyntax : SyntaxNode {
            var nextItem = new SyntaxAnalyzerGroupNode<TAnalyzer, TNewSyntax>(this, syntax => syntaxFactory((TSyntax)syntax));
            SetNextNode(nextItem);
            return nextItem;
        }

        public ISyntaxAnalyzerSetupGroup<TNewSyntax> ThenUseAnalyzer<TAnalyzer, TNewSyntax>(Func<TSyntax, IEnumerable<TNewSyntax>> syntaxFactory) where TAnalyzer : SyntaxNodeAnalyzer<TNewSyntax> where TNewSyntax : SyntaxNode {
            var nextItem = new SyntaxAnalyzerGroupNode<TAnalyzer, TNewSyntax>(this, syntax => syntaxFactory((TSyntax)syntax));
            SetNextNode(nextItem);
            return nextItem;
        }
    }

}
