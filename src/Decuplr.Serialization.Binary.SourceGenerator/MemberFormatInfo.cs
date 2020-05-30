using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal class MemberFormatInfo {
        public int Index { get; }
        public ISymbol MemberSymbol { get; }
        public ITypeSymbol MemberTypeSymbol { get; }
        public IReadOnlyList<AttributeData> Attributes { get; }

        public MemberFormatInfo(int index, ISymbol symbol) {
            Index = index;
            MemberSymbol = symbol;
            MemberTypeSymbol = symbol switch
            {
                IFieldSymbol fieldSymbol => fieldSymbol.Type,
                IPropertySymbol propertySymbol => propertySymbol.Type,
                _ => throw new ArgumentException("Symbol can only be field or property"),
            };
            // TODO : Read the attribute of the field or property to reveal lengh info and custom parser

        }
    }
}
