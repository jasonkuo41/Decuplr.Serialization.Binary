using System;

namespace Decuplr.CodeAnalysis {
    internal static class StringExtensions {

        internal static bool AnyEndsWith(this string str, string end)
            => str.AsSpan().TrimEnd().EndsWith(end.AsSpan());

        internal static bool AnyStartsWith(this string str, string start)
            => str.AsSpan().TrimStart().StartsWith(start.AsSpan());

        internal static bool AnyClampsWith(this string str, string start, string end)
            => str.AnyStartsWith(start) && str.AnyEndsWith(end);

        internal static string RemoveAfter(this string str, char character) {
            var index = str.IndexOf(character);
            if (index < 0)
                return str;
            return str.Substring(0, index);
        }
    }

}
