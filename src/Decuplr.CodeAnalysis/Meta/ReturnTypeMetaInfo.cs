using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Meta {
    public class ReturnTypeMetaInfo : ISymbolMetaInfo<ITypeSymbol> {

        private readonly ITypeSymbolProvider _provider;

        public ITypeSymbol Symbol { get; }
        public string Name => Symbol.Name;
        public bool IsVoid => Symbol.SpecialType == SpecialType.System_Void;

        ITypeSymbolProvider ISymbolMetaInfo<ITypeSymbol>.SymbolProvider => _provider;

        public ReturnTypeMetaInfo(ITypeSymbolProvider analysis, ITypeSymbol symbol) {
            _provider = analysis;
            Symbol = symbol;
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
