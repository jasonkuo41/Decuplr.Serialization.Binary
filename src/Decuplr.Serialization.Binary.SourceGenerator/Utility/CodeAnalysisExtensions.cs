using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

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

        public static TValue GetPropertyNamedArguement<TValue>(this AttributeData attribute, string name) {
            return (TValue)attribute.NamedArguments.First(x => x.Key == name).Value.Value;
        }

        public static bool HasAny(this ImmutableArray<AttributeData> data, INamedTypeSymbol symbol, SymbolEqualityComparer? comparer = null) {
            return data.Any(x => x.AttributeClass?.Equals(symbol, comparer ?? SymbolEqualityComparer.Default) ?? false);
        }

        public static ISymbol? GetSymbol<T>(this Compilation compilation) => compilation.GetTypeByMetadataName(typeof(T).FullName);

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

        public static void AddSource(this SourceGeneratorContext context, GeneratedSourceCode code, Encoding? encoding = null, bool shouldDumpFile = false) {
            context.AddSource(code.DesiredFileName, SourceText.From(code.SourceText, encoding ?? Encoding.UTF8));
            if (shouldDumpFile) {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), ".generated"));
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), ".generated", code.DesiredFileName), code.SourceText);
            }
        }

        public static T? GetNamedArgumentValue<T>(this AttributeData data, string propertyName) where T : struct {
            var value = data.NamedArguments.FirstOrDefault(x => x.Key == propertyName).Value.Value;
            if (value is null)
                return default;
            return (T)value;
        }

        public static T? GetNamedArgumentObject<T>(this AttributeData data, string propertyName) where T : class {
            var value = data.NamedArguments.FirstOrDefault(x => x.Key == propertyName).Value.Value;
            if (value is null)
                return default;
            return (T)value;
        }

        public static bool InheritFrom(this INamedTypeSymbol symbol, ITypeSymbol baseType) {
            while (symbol.BaseType != null) {
                if (symbol.BaseType.Equals(baseType, SymbolEqualityComparer.Default))
                    return true;
                symbol = symbol.BaseType;
            }
            return false;
        }

        public static bool Implements(this INamedTypeSymbol symbol, INamedTypeSymbol interfaceType) {
            return symbol.AllInterfaces.Any(x => x.Equals(interfaceType, SymbolEqualityComparer.Default));
        }

        public static bool InheritFrom<T>(this AnalyzedType type) => InheritFrom(type.TypeSymbol, type.Analyzer.GetSymbol<T>()!);
        public static bool Implements<T>(this AnalyzedType type) => Implements(type.TypeSymbol, type.Analyzer.GetSymbol<T>()!);
        public static bool Implements(this AnalyzedType type, Type interfaceType) => Implements(type.TypeSymbol, type.Analyzer.GetSymbol(interfaceType)!);
    }
}
