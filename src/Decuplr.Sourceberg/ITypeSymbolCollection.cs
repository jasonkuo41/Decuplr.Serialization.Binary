using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
    public interface ITypeSymbolCollection {
        Compilation DeclaringCompilation { get; }
        ITypeSymbol? GetSymbol<T>();
        ITypeSymbol? GetSymbol(Type type);
    }
}
