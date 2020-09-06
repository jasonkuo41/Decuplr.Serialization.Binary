using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
    public interface ITypeSymbolCollection {
        Compilation DeclaringCompilation { get; }
        INamedTypeSymbol? GetSymbol<T>();
        INamedTypeSymbol? GetSymbol(Type type);
        INamedTypeSymbol? GetSymbol(string metadataQualifyName);
    }
}
