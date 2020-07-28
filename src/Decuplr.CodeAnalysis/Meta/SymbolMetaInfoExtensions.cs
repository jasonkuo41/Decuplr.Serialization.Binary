using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Meta {
    public static class SymbolMetaInfoExtensions {

        public static IEnumerable<INamedTypeSymbol> GetInterfaces(this ISymbolMetaInfo<ITypeSymbol> meta, Type type) {
            if (!type.IsInterface)
                throw new ArgumentException(nameof(type));
            var typeSymbol = meta.SymbolProvider.GetSymbol(type);
            if (type.IsGenericTypeDefinition)
                return meta.Symbol.AllInterfaces.Where(x => x.IsGenericType || x.IsUnboundGenericType).Where(x => {
                    if (x.IsUnboundGenericType)
                        return x.Equals(typeSymbol, SymbolEqualityComparer.Default);
                    return x.ConstructUnboundGenericType().Equals(typeSymbol, SymbolEqualityComparer.Default);
                });
            return meta.Symbol.AllInterfaces.Where(x => x.Equals(typeSymbol, SymbolEqualityComparer.Default));
        }

        public static bool Implements(this ISymbolMetaInfo<ITypeSymbol> meta, Type type) => meta.GetInterfaces(type).Any();
        public static bool Implements<T>(this ISymbolMetaInfo<ITypeSymbol> meta) => meta.Implements(typeof(T));
        public static bool InheritFrom(this ISymbolMetaInfo<ITypeSymbol> meta, ITypeSymbol baseType) {
            ITypeSymbol symbol = meta.Symbol;
            while (symbol.BaseType != null) {
                if (symbol.BaseType.Equals(baseType, SymbolEqualityComparer.Default))
                    return true;
                symbol = symbol.BaseType;
            }
            return false;
        }

        public static bool InheritFrom<T>(this ISymbolMetaInfo<ITypeSymbol> meta) => meta.InheritFrom(meta.SymbolProvider.GetSymbol<T>()!);

    }
}
