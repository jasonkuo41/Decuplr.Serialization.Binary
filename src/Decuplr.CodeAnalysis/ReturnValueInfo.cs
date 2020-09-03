using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {
    public readonly struct ReturnValueInfo {
        public ITypeSymbol OriginalSymbol { get; }
        public TypeName TypeName { get; }

        public ReturnValueInfo(ITypeSymbol symbol) {
            OriginalSymbol = symbol;
            TypeName = TypeName.FromType(symbol);
        }

        internal static ReturnValueInfo? FromMember(ISymbol memberSymbol) {
            var returnType = (memberSymbol as IFieldSymbol)?.Type ?? (memberSymbol as IMethodSymbol)?.ReturnType ?? (memberSymbol as IPropertySymbol)?.Type ?? (memberSymbol as IEventSymbol)?.Type;
            if (returnType is null)
                return null;
            return new ReturnValueInfo(returnType);
        }
    }

}
