using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Services {
    public interface ITypeSymbolProvider {
        ITypeSymbolCollection Current { get; }
        ITypeSymbolCollection Source { get; }

        INamedTypeSymbol GetSourceSymbol(INamedTypeSymbol symbol);
    }
}
