using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.AnalyzeService {
    internal class MemberMetaInfo {

        private readonly IReadOnlyDictionary<AttributeData, Location> AttributeLocationLookup;

        public TypeMetaInfo ContainingFullType => ContainingType.Full;

        public TypePartialMetaInfo ContainingType { get; }

        public ISymbol MemberSymbol { get; }

        public ITypeSymbol? ReturnType { get; }

        public Location Location { get; }

        public IReadOnlyList<IReadOnlyList<AttributeData>> Attributes { get; }

        // We don't support this right now
        // Attributes for partial type where the attributes are attached to the declartion part (no content)
        // public IReadOnlyList<IReadOnlyList<AttributeData>> DeclaringAttributes { get; }

        public Location GetLocation(AttributeData data) {
            if (!AttributeLocationLookup.TryGetValue(data, out var location))
                throw new ArgumentException("Attribute is not found in the type", nameof(data));
            return location;
        }

        public MemberMetaInfo(TypePartialMetaInfo typemeta, ISymbol memberSymbol, MemberDeclarationSyntax syntax) {
            var listing = syntax.GetAttributes(memberSymbol);
            Location = syntax.GetLocation();
            ContainingType = typemeta;
            MemberSymbol = memberSymbol;
            ReturnType = (memberSymbol as IFieldSymbol)?.Type ?? (memberSymbol as IMethodSymbol)?.ReturnType ?? (memberSymbol as IPropertySymbol)?.Type ?? (memberSymbol as IEventSymbol)?.Type;
            Attributes = listing.Lists;
            AttributeLocationLookup = listing.Locations;

        }
    }

}
