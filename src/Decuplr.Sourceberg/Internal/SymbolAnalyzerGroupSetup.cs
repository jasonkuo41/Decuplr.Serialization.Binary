﻿using System.Collections.Generic;
using System.Collections.Immutable;
using Decuplr.Sourceberg.Generation;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Internal {
    internal class SymbolAnalyzerGroupSetup : ISymbolAnalyzerSetup {

        private readonly List<AnalyzerGroupInfo<ISymbol, SymbolKind>> _anaylzerGroups = new List<AnalyzerGroupInfo<ISymbol, SymbolKind>>();

        public IReadOnlyList<AnalyzerGroupInfo<ISymbol, SymbolKind>> AnalyzerGroups => _anaylzerGroups;

        public ISymbolAnalyzerSetupGroup<TSymbol> UseAnalyzer<TAnalyzer, TSymbol>(IEnumerable<SymbolKind> selectedSymbolKind)
            where TAnalyzer : SymbolActionAnalyzer<TSymbol>
            where TSymbol : ISymbol {
            var group = new AnalyzerGroup<ISymbol>();
            var first = new SymbolAnalyzerGroupNode<TAnalyzer, TSymbol>(group);
            _anaylzerGroups.Add((group, selectedSymbolKind.ToImmutableArray()));
            return first;
        }
    }

}
