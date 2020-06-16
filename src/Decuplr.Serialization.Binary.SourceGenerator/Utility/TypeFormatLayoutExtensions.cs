using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.Annotations.Internal;
using Microsoft.CodeAnalysis;
using Standart.Hash.xxHash;

namespace Decuplr.Serialization.Binary {
    internal static class TypeFormatLayoutExtensions {
        // This isn't good but it would do the job 
        private static readonly Regex ArrayRegex = new Regex(@"\[([\,\s]*|[^\]])\]", RegexOptions.Compiled);
        private static readonly Regex ValidNameRegex = new Regex(@"[^a-zA-Z0-9_]+", RegexOptions.Compiled);

        public static IEnumerable<string> GetConstructorMember(this TypeFormatLayout layout, bool containType) {
            for (var i = 0; i < layout.Member.Count; ++i) {
                var annotation = layout.Member[i].DecisionAnnotation;
                for (var j = 0; j < annotation.RequestParserType.Count; ++j) {
                    if (containType)
                        yield return $"{annotation.RequestParserType[j]} parser_{i}_{j}";
                    else
                        yield return $"parser_{i}_{j}";
                }
            }
        }

        public static string GetTypeParserContructor(this TypeFormatLayout layout) => string.Join(",", layout.GetConstructorMember(true));
        public static string GetTypeParserContructorInvokeParams(this TypeFormatLayout layout) => string.Join(",", layout.GetConstructorMember(false));

        [Obsolete("Use Template instead")]
        public static string GetDefaultParserCollectionName(this TypeFormatLayout layout) {
            if (!layout.TypeSymbol.IsGenericType)
                return $"{layout.TypeSymbol.GetEmbedName()}_TypeParserArgs";
            return $"{layout.TypeSymbol.GetEmbedName()}_TypeParserArgs<{string.Join(",", layout.TypeSymbol.TypeParameters.Select(x => x.ToString()))}>";
        }

        public static string GetDefaultAssemblyEntryClass(this Compilation compilation) {
            var assembly = compilation.Assembly;
            var symbol = compilation.GetTypeByMetadataName(typeof(DefaultParserAssemblyAttribute).FullName);
            var data = assembly.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
            if (data is null)
                return $"AssemblyFormatProvider_{ReplaceToValidVarName(assembly.Name)}_{assembly.ToString().HashString()}";
            return $"{((ITypeSymbol)data.ConstructorArguments[0].Value!).Name}";
        }

        public static bool IsDefaultAssembly(this Compilation compilation) {
            var symbol = compilation.GetTypeByMetadataName(typeof(DefaultParserAssemblyAttribute).FullName)!;
            return compilation.Assembly.GetAttributes().Any(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        private static string HashString(this string str) {
            var span = MemoryMarshal.Cast<char, byte>(str.AsSpan());
            var hashed = xxHash64.ComputeHash(span, span.Length);
            return ConvertChars(hashed);

            static string ConvertChars(ulong value) {
                const int UlongMaxChars = 20;
                const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                Span<char> result = stackalloc char[UlongMaxChars];
                var i = 0;
                while (value > 0) {
                    result[i++] = Chars[(int)(value % (ulong)Chars.Length)]; // use StringBuilder for better performance
                    value /= (ulong)Chars.Length;
                }

                return result.Slice(0, i).ToString();
            }
        }

        private static string ReplaceToValidVarName(string str) {
            str = ArrayRegex.Replace(str, "Array");
            str = ValidNameRegex.Replace(str, "_");
            return str;
        }

        /// <summary>
        /// Create's a name for a symbol that can easily identify class but with high uniqueness
        /// </summary>
        public static string GetEmbedName(this ITypeSymbol symbol) => $"{ReplaceToValidVarName(symbol.Name)}_{symbol.ToString().HashString()}";
    }

}
