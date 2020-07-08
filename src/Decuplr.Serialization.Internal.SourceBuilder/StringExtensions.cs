using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.SourceBuilder {
    static class StringExtensions {
        internal static bool ClampsWith(this string str, string start, string end) {
            var span = str.AsSpan().TrimStart().TrimEnd();
            return span.StartsWith(start.AsSpan()) && span.EndsWith(end.AsSpan());
        }
    }
}
