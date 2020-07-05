using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Standart.Hash.xxHash;

namespace Decuplr.Serialization.AnalysisService {
    internal static class SymbolNameExtensions {
        // This isn't good but it would do the job 
        private static readonly Regex ArrayRegex = new Regex(@"\[([\,\s]*|[^\]])\]", RegexOptions.Compiled);
        private static readonly Regex ValidNameRegex = new Regex(@"[^a-zA-Z0-9_]+", RegexOptions.Compiled);

        private static string HashString(this string str) {
            var span = MemoryMarshal.Cast<char, byte>(str.AsSpan());
            var hashed = xxHash64.ComputeHash(span, span.Length);
            return ConvertChars(hashed);

            static string ConvertChars(ulong value) {
                const int UlongMaxChars = 20;
                const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

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
        public static string GetUniqueIndetifier(this ISymbol symbol) => $"{ReplaceToValidVarName(symbol.Name)}_{symbol.ToString().HashString()}";
    }

}
