using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {


    public interface INamedTypeSource : ITypeMemberSource<INamedTypeSymbol> {
        IReadOnlyList<ITypeMemberSource> Members { get; }
        bool IsPartial { get; }
    }

    internal class NamedTypeSource : INamedTypeSource {

        public INamedTypeSymbol OriginalSymbol { get; }

        public IReadOnlyList<ITypeMemberSource> Members { get; }

        public IAttributeCollection Attributes { get; }

        public bool IsPartial => OriginalSymbol.Locations.Length > 1;

        public NamedTypeSource(INamedTypeSymbol symbol, ITypeSymbolProvider symbolProvider) {
            OriginalSymbol = symbol;
            Members = symbol.GetMembers().Select(x => new TypeMemberSource(this, x, symbolProvider)).ToList();

        }
    }

    public interface ITypeMemberSource {
        IAttributeCollection? Attributes { get; }
        INamedTypeSource ContainingType { get; }
        Location FirstLocation { get; }
        bool IsStatic { get; }
        string Name { get; }
        ISymbol OriginalSymbol { get; }
        ReturnValueInfo? ReturnValue { get; }
    }

    public interface ITypeMemberSource<out TSymbol> : ITypeMemberSource where TSymbol : ISymbol {
        new TSymbol OriginalSymbol { get; }
    }

    internal class TypeMemberSource : ITypeMemberSource<ISymbol> {
        public ISymbol OriginalSymbol { get; }

        public string Name => OriginalSymbol.Name;

        public bool IsStatic => OriginalSymbol.IsStatic;

        public INamedTypeSource ContainingType { get; }

        public Location FirstLocation => OriginalSymbol.Locations[0];

        public IAttributeCollection? Attributes { get; }

        public ReturnValueInfo? ReturnValue { get; }

        public TypeMemberSource(NamedTypeSource containingType, ISymbol memberSymbol, ITypeSymbolProvider symbolProvider) {
            ContainingType = containingType;
            OriginalSymbol = memberSymbol;
            ReturnValue = ReturnValueInfo.FromMember(memberSymbol);
            Attributes = AttributeCollection.GetAttributeLocations(symbolProvider, memberSymbol);
        }
    }

}
