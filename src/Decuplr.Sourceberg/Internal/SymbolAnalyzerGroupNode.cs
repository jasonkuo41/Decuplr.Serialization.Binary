using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Sourceberg.Generation;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Internal {
    internal class SymbolAnalyzerGroupNode<TSourceAnalyzer, TSymbol> : AnalyzerGroupNode<ISymbol>, ISymbolAnalyzerSetupGroup<TSymbol>
        where TSourceAnalyzer : SymbolActionAnalyzer<TSymbol>
        where TSymbol : ISymbol {

        public override Type AnalyzerType { get; } = typeof(TSourceAnalyzer);

        public SymbolAnalyzerGroupNode(AnalyzerGroup<ISymbol> group)
            : base(group) {
        }

        private SymbolAnalyzerGroupNode(AnalyzerGroupNode<ISymbol> node)
            : base(node.Group) {
        }

        private SymbolAnalyzerGroupNode(AnalyzerGroupNode<ISymbol> node, Func<ISymbol, ISymbol> syntaxFactory)
            : base(node.Group, syntaxFactory) {
        }

        private SymbolAnalyzerGroupNode(AnalyzerGroupNode<ISymbol> node, Func<ISymbol, IEnumerable<ISymbol>>? transitionMultiple)
            : base(node.Group, transitionMultiple) {
        }

        public ISymbolAnalyzerSetupGroup<TSymbol> ThenUseAnalyzer<TAnalyzer>() where TAnalyzer : SymbolActionAnalyzer<TSymbol> {
            var nextItem = new SymbolAnalyzerGroupNode<TAnalyzer, TSymbol>(this);
            SetNextNode(nextItem);
            return nextItem;
        }

        public ISymbolAnalyzerSetupGroup<TNewSymbol> ThenUseAnalyzer<TAnalyzer, TNewSymbol>(Func<TSymbol, TNewSymbol> syntaxFactory) where TAnalyzer : SymbolActionAnalyzer<TNewSymbol> where TNewSymbol : ISymbol {
            var nextItem = new SymbolAnalyzerGroupNode<TAnalyzer, TNewSymbol>(this, syntax => syntaxFactory((TSymbol)syntax));
            SetNextNode(nextItem);
            return nextItem;
        }

        public ISymbolAnalyzerSetupGroup<TNewSymbol> ThenUseAnalyzer<TAnalyzer, TNewSymbol>(Func<TSymbol, IEnumerable<TNewSymbol>> syntaxFactory) where TAnalyzer : SymbolActionAnalyzer<TNewSymbol> where TNewSymbol : ISymbol {
            var nextItem = new SymbolAnalyzerGroupNode<TAnalyzer, TNewSymbol>(this, syntax => syntaxFactory((TSymbol)syntax).Cast<ISymbol>());
            SetNextNode(nextItem);
            return nextItem;
        }
    }

}
