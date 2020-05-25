using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    // Represents an analyzed type
    internal class TypeFormatInfo {
        public INamedTypeSymbol TypeSymbol { get; }
        public IReadOnlyList<MemberFormatInfo> Members { get; }

        public IReadOnlyList<AttributeData> Attributes => TypeSymbol.GetAttributes();

        public TypeFormatInfo(INamedTypeSymbol typeSymbol, IReadOnlyList<MemberFormatInfo> member) {
            TypeSymbol = typeSymbol;
            Members = member;
        }
    }
}
