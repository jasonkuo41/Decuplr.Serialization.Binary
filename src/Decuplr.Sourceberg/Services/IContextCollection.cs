using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Services {
    public interface IContextCollection {
        ISourceFeatureCollection GetContextCollection(SyntaxNode syntax);
        ISourceFeatureCollection GetContextCollection(ISymbol symbol);
    }
}