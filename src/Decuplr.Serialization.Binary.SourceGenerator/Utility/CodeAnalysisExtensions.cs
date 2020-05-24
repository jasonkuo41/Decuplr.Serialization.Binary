using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    static class CodeAnalysisExtensions {
        /// <summary>
        /// Get's the full name of a class, without namespace
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string GetClassFullName(this INamedTypeSymbol symbol) {
            return string.Join(".", symbol.GetContainingTypes().Select(x => x.Name));
        }

        public static IEnumerable<INamedTypeSymbol> GetContainingTypes(this INamedTypeSymbol symbol) {
            var stack = new Stack<INamedTypeSymbol>();
            var currentSymbol = symbol;
            do {
                stack.Push(currentSymbol);
                currentSymbol = currentSymbol.ContainingType;
            } while (currentSymbol != null);
            return stack;
        }

        public static TValue GetArgument<TValue>(this AttributeData attribute, string name) {
            return (TValue)attribute.NamedArguments.First(x => x.Key == name).Value.Value;
        }

        /// <summary>
        /// Decides if a member can access a symbol internally within the generated class (without partial)
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool CanAccessSymbolInternally(this ISymbol member) {
            if (member is IFieldSymbol fieldSymbol)
                return CanFormatterAccess(fieldSymbol.DeclaredAccessibility);
            if (member is IPropertySymbol propSymbol)
                return !propSymbol.IsReadOnly && CanFormatterAccess(propSymbol.SetMethod.DeclaredAccessibility);
            return false;
        }

        private static bool CanFormatterAccess(Accessibility accessibility) => accessibility switch
        {
            Accessibility.Public => true,
            Accessibility.Internal => true,
            Accessibility.ProtectedOrInternal => true,
            Accessibility.ProtectedAndInternal => true,
            _ => false
        };
    }
}
