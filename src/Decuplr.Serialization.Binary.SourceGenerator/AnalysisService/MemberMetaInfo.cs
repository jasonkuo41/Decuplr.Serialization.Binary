using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.AnalysisService {
    internal class MemberMetaInfo {

        private readonly SourceCodeAnalysis _analysis;
        private readonly IReadOnlyDictionary<AttributeData, Location> _attributeLocationLookup;

        public bool IsStatic => MemberSymbol.IsStatic;

        public bool IsConst => MemberSymbol is IFieldSymbol fieldSymbol && fieldSymbol.IsConst;

        public TypeMetaInfo ContainingFullType => ContainingType.Full;

        public TypePartialMetaInfo ContainingType { get; }

        public ISymbol MemberSymbol { get; }

        public ITypeSymbol? ReturnType { get; }

        public Location Location { get; }

        public IReadOnlyList<IReadOnlyList<AttributeData>> Attributes { get; }

        public bool ContainsAttribute<TAttribute>() where TAttribute : Attribute {
            var symbol = _analysis.GetSymbol<TAttribute>();
            if (symbol is null)
                return false;
            return Attributes.SelectMany(x => x).Any(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        public AttributeData? GetAttribute<TAttribute>() where TAttribute : Attribute => GetAttributes<TAttribute>().FirstOrDefault();

        public IEnumerable<AttributeData> GetAttributes<TAttribute>() where TAttribute : Attribute {
            var symbol = _analysis.GetSymbol<TAttribute>();
            if (symbol is null)
                return Enumerable.Empty<AttributeData>();
            return Attributes.SelectMany(x => x).Where(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        // We don't support this right now
        // Attributes for partial type where the attributes are attached to the declartion part (no content)
        // public IReadOnlyList<IReadOnlyList<AttributeData>> DeclaringAttributes { get; }

        public Location GetLocation(AttributeData data) {
            if (!_attributeLocationLookup.TryGetValue(data, out var location))
                throw new ArgumentException("Attribute is not found in the type", nameof(data));
            return location;
        }

        public Location? GetLocation<TAttribute>() where TAttribute : Attribute => GetLocations<TAttribute>().FirstOrDefault();

        public IEnumerable<Location> GetLocations<TAttribute>() where TAttribute : Attribute {
            var symbol = _analysis.GetSymbol<TAttribute>();
            if (symbol is null)
                return Enumerable.Empty<Location>();
            return Attributes.SelectMany(x => x).Where(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false).Select(x => GetLocation(x));
        }

        public MemberMetaInfo(TypePartialMetaInfo typemeta, SourceCodeAnalysis analysis, ISymbol memberSymbol, MemberDeclarationSyntax syntax) {
            var listing = syntax.GetAttributes(memberSymbol);
            Location = syntax.GetLocation();
            ContainingType = typemeta;
            MemberSymbol = memberSymbol;
            ReturnType = (memberSymbol as IFieldSymbol)?.Type ?? (memberSymbol as IMethodSymbol)?.ReturnType ?? (memberSymbol as IPropertySymbol)?.Type ?? (memberSymbol as IEventSymbol)?.Type;
            Attributes = listing.Lists;
            _attributeLocationLookup = listing.Locations;
            _analysis = analysis;
        }

    }

}
