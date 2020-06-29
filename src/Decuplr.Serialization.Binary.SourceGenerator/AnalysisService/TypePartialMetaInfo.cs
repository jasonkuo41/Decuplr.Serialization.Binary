using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Serialization.Binary.AnalysisService {
    internal class TypePartialMetaInfo {

        private readonly IReadOnlyDictionary<AttributeData, Location> AttributeLocationLookup;

        public NamedTypeMetaInfo Full { get; }

        public Location Location { get; }

        public IReadOnlyList<IReadOnlyList<AttributeData>> Attributes { get; }

        public IReadOnlyList<MemberMetaInfo> Members { get; }

        public TypePartialMetaInfo(NamedTypeMetaInfo source, SyntaxModelPair syntaxPair, IReadOnlyList<SymbolKind> kinds, CancellationToken ct) {
            var attributeListing = syntaxPair.Syntax.GetAttributes(source.Symbol);
            Full = source;
            Location = syntaxPair.Syntax.GetLocation();
            Attributes = attributeListing.Lists;
            AttributeLocationLookup = attributeListing.Locations;
            Members = GetUnderlyingMembers();

            IReadOnlyList<MemberMetaInfo> GetUnderlyingMembers() {
                var members = new List<MemberMetaInfo>();
                foreach (var member in syntaxPair.Syntax.Members) {
                    var symbol = syntaxPair.Model.GetDeclaredSymbol(member, ct);
                    // Filter out null symbols and symbols that are not enlisted for interest
                    if (symbol is null || !kinds.Contains(symbol.Kind))
                        continue;
                    // We don't support partial method, so we ignore symbols that are not impl.
                    // In future versions when we have use cases that can benefit from partial, then we should consider adding Attributes to the DefinitionPart
                    // Adding to the commented "DeclaringAttribute" part
                    if (symbol is IMethodSymbol method && method.PartialImplementationPart != null && method.PartialDefinitionPart is null && member.Modifiers.Any(SyntaxKind.PartialKeyword)) {
                        continue;
                    }
                    members.Add(new MemberMetaInfo(this, symbol, member));
                }
                return members;
            }
        }

        public Location GetLocation(AttributeData data) {
            if (!AttributeLocationLookup.TryGetValue(data, out var location))
                throw new ArgumentException("Attribute is not found in the type");
            return location;
        }
    }

}
