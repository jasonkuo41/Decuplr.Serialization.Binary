using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {
    public static class TypeSymbolExtensions {

        internal static IEnumerable<Type> GetAllDeclaringTypes(this Type type) {
            return UnwrapParentType(type).Reverse();

            static IEnumerable<Type> UnwrapParentType(Type type) {
                var parent = type.DeclaringType;
                while (parent is { }) {
                    yield return parent;
                    parent = parent.DeclaringType;
                }
            }
        }

        public static TypeName ToTypeName(this ITypeSymbol symbol) => TypeName.FromType(symbol);
    }
}
