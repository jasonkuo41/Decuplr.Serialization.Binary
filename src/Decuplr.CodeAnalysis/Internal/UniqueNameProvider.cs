using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Standart.Hash.xxHash;

namespace Decuplr.CodeAnalysis.Internal {
    internal class UniqueNameProvider : IUniqueNameProvider {

        // This isn't good but it would do the job 
        private static readonly Regex ArrayRegex = new Regex(@"\[([\,\s]*|[^\]])\]", RegexOptions.Compiled);
        private static readonly Regex ValidNameRegex = new Regex(@"[^a-zA-Z0-9_]+", RegexOptions.Compiled);

        private static string Hash(string str) {
            var span = MemoryMarshal.Cast<char, byte>(str.AsSpan());
            var hashed = xxHash64.ComputeHash(span, span.Length);
            return ConvertChars(hashed);
        }

        private static string ConvertChars(ulong value) {
            const int UlongMaxChars = 20;
            const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            Span<char> result = stackalloc char[UlongMaxChars];
            var i = 0;
            while (value > 0) {
                result[i++] = Chars[(int)(value % (ulong)Chars.Length)];
                value /= (ulong)Chars.Length;
            }

            return result.Slice(0, i).ToString();
        }

        private static string ReplaceInvalidName(string str) {
            str = ArrayRegex.Replace(str, "Array");
            str = ValidNameRegex.Replace(str, "_");
            return str;
        }

        public string GetUniqueName(ISymbol symbol) {
            return $"{ReplaceInvalidName(symbol.Name)}_{Hash(symbol.ToString())}";
        }
    }
}
