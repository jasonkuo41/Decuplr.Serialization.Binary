using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Sourceberg.Services;
using Decuplr.Sourceberg.Services.Implementation;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {

    internal class AttributeLayoutProvider : IAttributeLayoutProvider {

        private readonly ITypeSymbolProvider _symbolProvider;
        private readonly TypeSymbolLocatorCache _locatorCache;
        private readonly SimpleCacheDictionary<ISymbol, IAttributeCollection?> _cache = new SimpleCacheDictionary<ISymbol, IAttributeCollection?>(2048);

        public AttributeLayoutProvider(ITypeSymbolProvider symbolProvider, TypeSymbolLocatorCache locatorCache) {
            _symbolProvider = symbolProvider;
            _locatorCache = locatorCache;
        }

        private IAttributeCollection? CreateCollection(ISymbol symbol) => AttributeCollection.CreateCollection(_symbolProvider, _locatorCache, symbol);

        public IAttributeCollection? GetAttributes(ISymbol symbol) => _cache.GetOrAdd(symbol, CreateCollection);
    }
}
