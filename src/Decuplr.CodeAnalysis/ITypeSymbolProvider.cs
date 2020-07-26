using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {
    public interface ITypeSymbolProvider {
        INamedTypeSymbol GetSymbol<T>();
        INamedTypeSymbol GetSymbol(Type type);
        INamedTypeSymbol GetSymbol(string fullName);
        bool TryGetSymbol(Type type, out INamedTypeSymbol? symbol);
        bool TryGetSymbol<T>(out INamedTypeSymbol? symbol);
        bool TryGetSymbol(string fullName, out INamedTypeSymbol? symbol);
    }

}
