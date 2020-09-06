using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Sourceberg.Generation {
    public struct GeneratingContext {
        public void RequireGenerateWithSyntax<TSyntax>(TSyntax syntax) where TSyntax : SyntaxNode {
            var tsyntax = syntax as TypeDeclarationSyntax;
            tsyntax.
        }

        public void RequireGenerateWithSymbol<TSymbol>(TSymbol symbol) where TSymbol : ISymbol {

        }
    }
}
