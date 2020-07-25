using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decuplr.Serialization.AnalysisService;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Meta {
    public class ReturnTypeMetaInfo : BaseTypeMetaInfo<ITypeSymbol> {

        public string Name => Symbol.Name;
        public bool IsVoid => Symbol.SpecialType == SpecialType.System_Void;

        public ReturnTypeMetaInfo(ITypeSymbolProvider analysis, ITypeSymbol symbol)
            : base(analysis, symbol) {
        }

        public static ReturnTypeMetaInfo? FromMember(ITypeSymbolProvider analysis, ISymbol memberSymbol) {
            var returnType = (memberSymbol as IFieldSymbol)?.Type ?? (memberSymbol as IMethodSymbol)?.ReturnType ?? (memberSymbol as IPropertySymbol)?.Type ?? (memberSymbol as IEventSymbol)?.Type;
            if (returnType is null)
                return null;
            return new ReturnTypeMetaInfo(analysis, returnType);
        }

        public override string ToString() => Symbol.ToString();

    }
}
