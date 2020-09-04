using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {
    internal class NamedTypeSource : TypeMemberSource, INamedTypeSource {

        private readonly INamedTypeSymbol _originalSymbol;

        public TypeName TypeName { get; }

        public IReadOnlyList<ITypeMemberSource> Members { get; }

        public bool IsPartial => OriginalSymbol.Locations.Length > 1;

        INamedTypeSymbol ITypeMemberSource<INamedTypeSymbol>.OriginalSymbol => _originalSymbol;

        public NamedTypeSource(INamedTypeSource? parent, INamedTypeSymbol symbol, ITypeSymbolProvider symbolProvider)
            : base(parent, symbol, symbolProvider) {
            _originalSymbol = symbol;
            TypeName = TypeName.FromType(_originalSymbol);
            Members = symbol.GetMembers().Select(memberSymbol => memberSymbol.CreateSource(symbolProvider, this)).ToList();
        }

    }

}
