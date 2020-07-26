using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public class SchemaLayout : IEquatable<SchemaLayout> {

        /// <summary>
        /// The type that this layout represents
        /// </summary>
        public NamedTypeMetaInfo Type { get; }

        /// <summary>
        /// The layout members, in order
        /// </summary>
        public IReadOnlyList<MemberMetaInfo> Members { get; }

        public SchemaLayout(NamedTypeMetaInfo type, IReadOnlyList<MemberMetaInfo> typeMembers) {
            if (typeMembers.Any(x => x.ContainingFullType != type))
                throw new ArgumentException($"Type Members '{string.Join(",", typeMembers.Where(x => x.ContainingFullType != type))}' must be a member of '{type.Symbol}' ");
            Type = type;
            Members = typeMembers;
        }

        public SchemaLayout MakeGenericType(params ITypeSymbol[] symbols) {
            // Poor performance, if compile speed is too slow we can look at this
            var type = Type.MakeGenericType(symbols);
            var typeMember = GetReordered(type).ToList();
            return new SchemaLayout(type, typeMember);

            IEnumerable<MemberMetaInfo> GetReordered(NamedTypeMetaInfo type) {
                // We look up each layout and make sure that they are the similar instance (but different symbol owner)
                foreach (var layout in Members) {
                    foreach (var targetLayout in type.Members) {
                        if (layout.Location.Equals(targetLayout.Location))
                            yield return targetLayout;
                    }
                }
            }
        }

        public override int GetHashCode() {
            var hash = new HashCode();
            hash.Add(Type);
            for (var i = 0; i < Members.Count; ++i)
                hash.Add(Members[i]);
            return hash.ToHashCode();
        }

        public override bool Equals(object obj) => obj is SchemaLayout layout && Equals(layout);

        public bool Equals(SchemaLayout layout) {
            if (layout.Members.Count != Members.Count)
                return false;

            if (!Type.Equals(layout.Type))
                return false;

            for (int i = 0; i < Members.Count; i++) {
                if (!Members[i].Equals(layout.Members[i]))
                    return false;
            }
            return true;
        }

    }
}
