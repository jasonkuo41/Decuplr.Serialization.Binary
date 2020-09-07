using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Services {
    public interface ITypeSymbolProvider {
        ITypeSymbolCollection Current { get; }
        ITypeSymbolCollection Source { get; }

        // TODO : Implement GetSourceSymbol
        //INamedTypeSymbol GetSourceSymbol(INamedTypeSymbol symbol);
    }
}
