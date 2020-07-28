using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public class SchemaLayout {

        /// <summary>
        /// The type that this layout represents
        /// </summary>
        public NamedTypeMetaInfo Type { get; }

        /// <summary>
        /// The layout members, in order
        /// </summary>
        public IReadOnlyList<MemberMetaInfo> Members { get; }

        /// <summary>
        /// The info of this schema
        /// </summary>
        public SchemaInfo LayoutInfo { get; }

        public SchemaLayout(SchemaInfo schemaInfo, IReadOnlyList<MemberMetaInfo> typeMembers) {
            var type = schemaInfo.SourceTypeInfo;
            if (typeMembers.Any(x => x.ContainingFullType != type))
                throw new ArgumentException($"Type Members '{string.Join(",", typeMembers.Where(x => x.ContainingFullType != type))}' must be a member of '{type.Symbol}' ");
            Type = type;
            Members = typeMembers;
            LayoutInfo = schemaInfo;
        }

        public SchemaLayout MakeGenericType(params ITypeSymbol[] symbols) {
            // Poor performance, if compile speed is too slow we can look at this
            var layout = LayoutInfo.MakeGenericType(symbols);
            var typeMember = GetReordered(layout.SourceTypeInfo).ToList();
            return new SchemaLayout(layout, typeMember);

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

    }
}
