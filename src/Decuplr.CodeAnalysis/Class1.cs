using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {

    public interface INamedTypeSource : ITypeMemberSource<INamedTypeSymbol> {
        TypeName TypeName { get; }
        IReadOnlyList<ITypeMemberSource> Members { get; }
        bool IsPartial { get; }
    }

    internal static class SourceExtensions {
        public static ITypeMemberSource CreateSource(this ISymbol symbol, ITypeSymbolProvider symbolProvider, INamedTypeSource? parent) => symbol switch
        {
            INamedTypeSymbol typeSymbol => new NamedTypeSource(parent, typeSymbol.WithNullableAnnotation(NullableAnnotation.Annotated), symbolProvider),
            _ => new TypeMemberSource(parent, symbol, symbolProvider)
        };
    }

    public interface ITypeMemberSource : IEquatable<ITypeMemberSource> {
        IAttributeCollection? Attributes { get; }
        INamedTypeSource? ContainingType { get; }
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

        public INamedTypeSource? ContainingType { get; }

        public Location FirstLocation => OriginalSymbol.Locations[0];

        public IAttributeCollection? Attributes { get; }

        public ReturnValueInfo? ReturnValue { get; }

        public TypeMemberSource(INamedTypeSource? containingType, ISymbol memberSymbol, ITypeSymbolProvider symbolProvider) {
            ContainingType = containingType;
            OriginalSymbol = memberSymbol;
            ReturnValue = ReturnValueInfo.FromMember(memberSymbol);
            Attributes = AttributeCollection.GetAttributeLocations(symbolProvider, memberSymbol);
        }

        public bool Equals(ITypeMemberSource other) => OriginalSymbol.Equals(other.OriginalSymbol, SymbolEqualityComparer.Default);
        public override bool Equals(object obj) => obj is ITypeMemberSource source && Equals(source);
        public override int GetHashCode() => OriginalSymbol.GetHashCode();
    }

}
