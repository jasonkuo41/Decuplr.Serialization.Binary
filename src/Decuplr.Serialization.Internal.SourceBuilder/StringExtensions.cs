using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.SourceBuilder {
    static class StringExtensions {

        internal static bool AnyEndsWith(this string str, string end) 
            => str.AsSpan().TrimEnd().EndsWith(end.AsSpan());

        internal static bool AnyStartsWith(this string str, string start) 
            => str.AsSpan().TrimStart().StartsWith(start.AsSpan());

        internal static bool AnyClampsWith(this string str, string start, string end)
            => str.AnyStartsWith(start) && str.AnyEndsWith(end);

    }
}
