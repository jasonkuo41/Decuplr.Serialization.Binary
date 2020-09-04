using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
    internal class AttributeCollection : IAttributeCollection {

        private class LocationComparedBySpan : IComparer<Location> {
            private LocationComparedBySpan() { }
            public int Compare(Location x, Location y) {
                if (x is null && y is null)
                    return 0;
                if (x is null || y is null || !x.Equals(y))
                    return 0;
                return x.SourceSpan.CompareTo(y.SourceSpan);
            }
            public static LocationComparedBySpan Instance { get; } = new LocationComparedBySpan();
        }

        private static Dictionary<Location, IReadOnlyList<AttributeLayout>> EmptyLocations { get; } = new Dictionary<Location, IReadOnlyList<AttributeLayout>>();

        private readonly ITypeSymbolProvider _symbolProvider;
        private readonly ImmutableArray<AttributeData> _attributes;

        public IReadOnlyList<AttributeLayout> Attributes { get; }

        public IReadOnlyDictionary<Location, IReadOnlyList<AttributeLayout>> AttributeLocations { get; }

        public ISymbol ContainingSymbol { get; }

        private AttributeCollection(ITypeSymbolProvider provider, ISymbol sourceSymbol) {
            _symbolProvider = provider;
            ContainingSymbol = sourceSymbol;
            _attributes = sourceSymbol.GetAttributes();
            (AttributeLocations, Attributes) = GetAttributeLocations();
        }

        public static AttributeCollection? GetAttributeLocations(ITypeSymbolProvider provider, ISymbol sourceSymbol) {
            if (!sourceSymbol.Locations.All(x => x.IsInSource))
                return null;
            return new AttributeCollection(provider, sourceSymbol);
        }

        private (IReadOnlyDictionary<Location, IReadOnlyList<AttributeLayout>> Locations, IReadOnlyList<AttributeLayout> Attributes) GetAttributeLocations() {
            if (!ContainingSymbol.Locations.All(x => x.IsInSource))
                return (EmptyLocations, _attributes.Select(x => new AttributeLayout(x, null)).ToList());

            var locations = ContainingSymbol.Locations.Sort(LocationComparedBySpan.Instance);
            var dict = locations.ToDictionary(x => x, x => new List<AttributeData>());

            foreach (var attribute in _attributes) {
                var hasAdded = false;
                foreach (var (location, list) in dict) {
                    Debug.Assert(attribute.ApplicationSyntaxReference is { });
                    Debug.Assert(location.SourceTree is { });
                    var (currentSyntaxTree, currentSpan) = (attribute.ApplicationSyntaxReference.SyntaxTree, attribute.ApplicationSyntaxReference.Span);

                    if (!location.SourceTree.Equals(currentSyntaxTree))
                        continue;
                    // We know that attribute must be smaller then the source span
                    // and since the locations are sorted by it's relative span, we can safely assume that once it's smaller, it's the first instance to be smaller
                    if (currentSpan.CompareTo(location.SourceSpan) >= 0)
                        continue;
                    hasAdded = true;
                    list.Add(attribute);
                    break;
                }
                Debug.Assert(hasAdded);
            }

            // Finally we sort the dictionary
            var result = dict.ToDictionary(x => x.Key, x => (IReadOnlyList<AttributeLayout>)x.Value.OrderBy(x => x).Select(attr => new AttributeLayout(attr, x.Key)).ToList());
            result.RemoveWhere(x => x.Value.Count == 0);
            return (result, result.SelectMany(x => x.Value).ToList());
        }

        public bool ContainsAttribute<TAttribute>() where TAttribute : Attribute => ContainsAttribute(typeof(TAttribute));

        public bool ContainsAttribute(Type attributeType) {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new InvalidOperationException($"{attributeType} is not a type of Attribute");
            var symbol = _symbolProvider.GetSymbol(attributeType);
            if (symbol is null)
                return false;
            return _attributes.Any(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        public AttributeLayout<TAttribute>? GetAttribute<TAttribute>() where TAttribute : Attribute => GetAttribute(typeof(TAttribute))?.AsInfo<TAttribute>();

        public AttributeLayout? GetAttribute(Type attributeType) => GetAttributes(attributeType).FirstOrDefault();

        public IEnumerable<AttributeLayout<TAttribute>> GetAttributes<TAttribute>() where TAttribute : Attribute => GetAttributes(typeof(TAttribute)).Select(x => x.AsInfo<TAttribute>());

        public IEnumerable<AttributeLayout> GetAttributes(Type attributeType) {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new InvalidOperationException($"{attributeType} is not a type of Attribute");
            var symbol = _symbolProvider.GetSymbol(attributeType);
            if (symbol is null)
                return Enumerable.Empty<AttributeLayout>();
            return Attributes.Where(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

    }

}
