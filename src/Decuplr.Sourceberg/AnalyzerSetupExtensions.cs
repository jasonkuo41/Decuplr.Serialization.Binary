using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.Sourceberg.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Sourceberg {
    public static class AnalyzerSetupExtensions {
        public static ISyntaxAnalyzerSetupGroup<TSyntax> UseAnalyzer<TAnalyzer, TSyntax>(this ISyntaxAnalyzerSetup setup, SyntaxKind firstSelectedSyntaxKind, params SyntaxKind[] selectedSyntaxKinds)
            where TAnalyzer : SyntaxNodeAnalyzer<TSyntax>
            where TSyntax : SyntaxNode {
            var selected = selectedSyntaxKinds.Prepend(firstSelectedSyntaxKind);
            return setup.UseAnalyzer<TAnalyzer, TSyntax>(selected);
        }

        public static ISymbolAnalyzerSetupGroup<TSymbol> UseAnalyzer<TAnalyzer, TSymbol>(this ISymbolAnalyzerSetup setup, SymbolKind firstSelectedSymbolKind, params SymbolKind[] selectedSymbolKinds) 
            where TAnalyzer : SymbolActionAnalyzer<TSymbol>
            where TSymbol : ISymbol {
            var selected = selectedSymbolKinds.Prepend(firstSelectedSymbolKind);
            return setup.UseAnalyzer<TAnalyzer, TSymbol>(selected);
        }
    }
}
