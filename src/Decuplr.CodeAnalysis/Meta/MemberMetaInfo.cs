using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.AnalysisService;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis.Meta {
    public class MemberMetaInfo : IEquatable<MemberMetaInfo> {

        private readonly ITypeSymbolProvider _analysis;
        private readonly IReadOnlyDictionary<AttributeData, Location> _attributeLocation;

        public bool IsStatic => Symbol.IsStatic;

        public string Name => Symbol.Name;

        public bool IsConst => Symbol is IFieldSymbol fieldSymbol && fieldSymbol.IsConst;

        public NamedTypeMetaInfo ContainingFullType => ContainingType.Full;

        public TypePartialMetaInfo ContainingType { get; }

        public ISymbol Symbol { get; }

        public ReturnTypeMetaInfo? ReturnType { get; }

        public Location Location { get; }

        public IReadOnlyList<IReadOnlyList<AttributeData>> Attributes { get; }

        internal MemberMetaInfo(TypePartialMetaInfo typemeta, MemberDeclarationSyntax syntax, ITypeSymbolProvider analysis, ISymbol memberSymbol) {
            (Attributes, _attributeLocation) = syntax.GetAttributes(memberSymbol);
            Location = syntax.GetLocation();
            ContainingType = typemeta;
            Symbol = memberSymbol;
            ReturnType = ReturnTypeMetaInfo.FromMember(analysis, memberSymbol);

            _analysis = analysis;
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
            if (!_attributeLocation.TryGetValue(data, out var location))
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

        public bool Equals(MemberMetaInfo other) => ContainingFullType.Equals(other.ContainingFullType) && Location.Equals(other.Location);
        public override bool Equals(object obj) => obj is MemberMetaInfo memberInfo && Equals(memberInfo);
        public override int GetHashCode() => HashCode.Combine(ContainingFullType, Location);
    }

}
