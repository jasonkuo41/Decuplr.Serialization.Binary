using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Services {
    public interface IContextCollectionProvider {
        IContextCollection GetContextCollection(SyntaxNode syntax);
        IContextCollection GetContextCollection(ISymbol symbol);
    }
}