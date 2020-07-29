using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Decuplr.CodeAnalysis.Meta.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.Meta {
    public class TypePartialMetaInfo {

        private readonly IReadOnlyDictionary<AttributeData, Location> _attributeLookup;

        public NamedTypeMetaInfo Full { get; }

        public Location Location { get; }

        public IReadOnlyList<IReadOnlyList<AttributeData>> Attributes { get; }

        public IReadOnlyList<MemberMetaInfo> Members { get; }

        internal TypePartialMetaInfo(NamedTypeMetaInfo source, ITypeSymbolProvider analysis, SyntaxModelPair syntaxPair, Func<ISymbol, bool>? memberPredicate, CancellationToken ct) {
            var attributeListing = syntaxPair.Syntax.GetAttributes(source.Symbol);
            _attributeLookup = attributeListing.Locations;

            Full = source;
            Location = syntaxPair.Syntax.GetLocation();
            Attributes = attributeListing.Lists;
            Members = GetUnderlyingMembers();

            IReadOnlyList<MemberMetaInfo> GetUnderlyingMembers() {
                var members = new List<MemberMetaInfo>();
                foreach (var member in syntaxPair.Syntax.Members) {
                    var symbol = syntaxPair.Model.GetDeclaredSymbol(member, ct);
                    // Filter out null symbols and symbols that are not enlisted for interest
                    if (symbol is null || !(memberPredicate?.Invoke(symbol) ?? true))
                        continue;
                    // We don't support partial method, so we ignore symbols that are not impl.
                    // In future versions when we have use cases that can benefit from partial, then we should consider adding Attributes to the DefinitionPart
                    // Adding to the commented "DeclaringAttribute" part
                    if (symbol is IMethodSymbol method && method.PartialImplementationPart != null && method.PartialDefinitionPart is null && member.Modifiers.Any(SyntaxKind.PartialKeyword)) {
                        continue;
                    }
                    members.Add(new MemberMetaInfo(this, member, analysis, symbol));
                }
                return members;
            }
        }

        internal TypePartialMetaInfo(NamedTypeMetaInfo parent, TypePartialMetaInfo source, INamedTypeSymbol newSymbol) {
            if (!newSymbol.ConstructedFrom.Equals(source.Full.Symbol, SymbolEqualityComparer.Default))
                throw new ArgumentException($"New symbol '{newSymbol}' is not a construction of the generic definition from the source symbol '{source.Full.Symbol}'", nameof(newSymbol));
            var memberLookup = newSymbol.GetMembers().ToDictionary(x => x.OriginalDefinition, x => x, SymbolEqualityComparer.Default);
            _attributeLookup = source._attributeLookup;

            Full = parent;
            Location = source.Location;
            Attributes = source.Attributes;
            Members = source.Members.Select(x => new MemberMetaInfo(this, x, memberLookup[x.Symbol])).ToList();
        }

        public Location GetLocation(AttributeData data) {
            if (!_attributeLookup.TryGetValue(data, out var location))
                throw new ArgumentException("Attribute is not found in the type");
            return location;
        }
    }

}
