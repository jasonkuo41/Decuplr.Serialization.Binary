using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
    internal static class Utilities {
        public static void ThrowIfNull<T>(this T item, string name) where T : class {
            if (item is null)
                throw new NullReferenceException(name);
        }

        public static IEnumerable<Type> GetAllDeclaringTypes(this Type type) {
            return UnwrapParentType(type).Reverse();

            static IEnumerable<Type> UnwrapParentType(Type type) {
                var parent = type.DeclaringType;
                while (parent is { }) {
                    yield return parent;
                    parent = parent.DeclaringType;
                }
            }
        }

        public static IReadOnlyList<int> GetArities(this Type type) {
            var typeWithParents = GetAllDeclaringTypesWithSelf(type);
            var layouts = new int[typeWithParents.Count];
            var currentSum = 0;
            for (var i = 0; i < typeWithParents.Count; ++i) {
                var length = typeWithParents[i].GetGenericArguments().Length - currentSum;
                layouts[i] = length;
                currentSum += length;
            }
            return layouts;

            static IReadOnlyList<Type> GetAllDeclaringTypesWithSelf(Type type) {
                var list = type.GetAllDeclaringTypes().ToList();
                list.Add(type);
                return list;
            }
        }

    }

}
