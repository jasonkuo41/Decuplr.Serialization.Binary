using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    public static class TypeNameExtensions {
        public static TypeName ToTypeName(this ITypeSymbol symbol) => new TypeName(symbol);
    }
}
