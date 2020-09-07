using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Services.Implementation {
    internal class ContextCollectionProvider : IContextCollectionProvider {

        private readonly Dictionary<ISymbol, IContextCollection> _symbolContextCollection = new Dictionary<ISymbol, IContextCollection>();
        private readonly Dictionary<SyntaxNode, IContextCollection> _syntaxContextCollection = new Dictionary<SyntaxNode, IContextCollection>();

        public IContextCollection GetContextCollection(SyntaxNode syntax) {
            if (_syntaxContextCollection.TryGetValue(syntax, out var value))
                return value;
            value = new ContextCollection();
            _syntaxContextCollection[syntax] = value;
            return value;
        }

        public IContextCollection GetContextCollection(ISymbol symbol) {
            if (_symbolContextCollection.TryGetValue(symbol, out var value))
                return value;
            value = new ContextCollection();
            _symbolContextCollection[symbol] = value;
            return value;
        }
    }

}
