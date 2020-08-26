using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    static class LinqExtensions {
        public static void ThrowAny<TKind>(this IEnumerable<TKind> enumerable, Func<IEnumerable<TKind>, Exception> onFound) {
            if (enumerable.Any())
                throw onFound(enumerable);
        }
    }
}
