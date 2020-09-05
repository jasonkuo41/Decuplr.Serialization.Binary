using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
    public interface ITypeSymbolProvider {
        ITypeSymbolCollection Current { get; }
        ITypeSymbolCollection Source { get; }

        INamedTypeSymbol GetSourceSymbol(INamedTypeSymbol symbol);
    }

    public interface ITypeSymbolCollection {
        Compilation DeclaringCompilation { get; }
        INamedTypeSymbol? GetSymbol<T>();
        INamedTypeSymbol? GetSymbol(Type type);
        INamedTypeSymbol? GetSymbol(string metadataQualifyName);
    }
}
