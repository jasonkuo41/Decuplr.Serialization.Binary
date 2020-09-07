using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Services.Implementation {
    class TypeSymbolLocatorCache {

        private readonly SimpleCacheDictionary<Compilation, ReflectionTypeSymbolLocator> _locaters = new SimpleCacheDictionary<Compilation, ReflectionTypeSymbolLocator>(256);

        public ReflectionTypeSymbolLocator GetLocator(Compilation compilation) 
            => _locaters.GetOrAdd(compilation, compilation => new ReflectionTypeSymbolLocator(compilation));

    }
}
