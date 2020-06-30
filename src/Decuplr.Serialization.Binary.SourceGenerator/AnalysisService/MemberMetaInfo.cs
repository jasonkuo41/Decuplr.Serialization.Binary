using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.AnalysisService {
    internal class MemberMetaInfo {

        private readonly SourceCodeAnalysis _analysis;
        private readonly IReadOnlyDictionary<AttributeData, Location> _attributeLocationLookup;

        public bool IsStatic => Symbol.IsStatic;

        public bool IsConst => Symbol is IFieldSymbol fieldSymbol && fieldSymbol.IsConst;

        public NamedTypeMetaInfo ContainingFullType => ContainingType.Full;

        public TypePartialMetaInfo ContainingType { get; }

        public ISymbol Symbol { get; }

        public ReturnTypeMetaInfo? ReturnType { get; }

        public Location Location { get; }

        public IReadOnlyList<IReadOnlyList<AttributeData>> Attributes { get; }

        public MemberMetaInfo(TypePartialMetaInfo typemeta, SourceCodeAnalysis analysis, ISymbol memberSymbol, MemberDeclarationSyntax syntax) {
            var listing = syntax.GetAttributes(memberSymbol);
            Location = syntax.GetLocation();
            ContainingType = typemeta;
            Symbol = memberSymbol;
            ReturnType = GetMetaInfo((memberSymbol as IFieldSymbol)?.Type ?? (memberSymbol as IMethodSymbol)?.ReturnType ?? (memberSymbol as IPropertySymbol)?.Type ?? (memberSymbol as IEventSymbol)?.Type);
            Attributes = listing.Lists;
            _attributeLocationLookup = listing.Locations;
            _analysis = analysis;

            ReturnTypeMetaInfo? GetMetaInfo(ITypeSymbol? symbol) => symbol is null ? null : new ReturnTypeMetaInfo(analysis, symbol);
        }

        public bool ContainsAttribute<TAttribute>() where TAttribute : Attribute => ContainsAttribute(typeof(TAttribute));

        public bool ContainsAttribute(Type attributeType) {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new InvalidOperationException($"{attributeType} is not a type of Attribute");
            var symbol = _analysis.GetSymbol(attributeType);
            if (symbol is null)
                return false;
            return Attributes.SelectMany(x => x).Any(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        public AttributeData? GetAttribute<TAttribute>() where TAttribute : Attribute => GetAttributes<TAttribute>().FirstOrDefault(); 
        public AttributeData? GetAttribute(Type attributeType) => GetAttributes(attributeType).FirstOrDefault();

        public IEnumerable<AttributeData> GetAttributes<TAttribute>() where TAttribute : Attribute => GetAttributes(typeof(TAttribute));

        public IEnumerable<AttributeData> GetAttributes(Type attributeType) {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new InvalidOperationException($"{attributeType} is not a type of Attribute");
            var symbol = _analysis.GetSymbol(attributeType);
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

        public Location? GetLocation(Type attributeType) => GetLocations(attributeType).FirstOrDefault();

        public IEnumerable<Location> GetLocations<TAttribute>() where TAttribute : Attribute => GetLocations(typeof(TAttribute));

        public IEnumerable<Location> GetLocations(Type attributeType) {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new InvalidOperationException($"{attributeType} is not a type of Attribute");
            var symbol = _analysis.GetSymbol(attributeType);
            if (symbol is null)
                return Enumerable.Empty<Location>();
            return Attributes.SelectMany(x => x).Where(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false).Select(x => GetLocation(x));
        }

    }

}
