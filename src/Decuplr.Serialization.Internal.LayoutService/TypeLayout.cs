using System;
using System.Collections.Generic;
using Decuplr.Serialization.AnalysisService;

namespace Decuplr.Serialization.LayoutService {
    public class TypeLayout : IEquatable<TypeLayout> {

        public NamedTypeMetaInfo Type { get; }
        public IReadOnlyList<MemberMetaInfo> Layouts { get; }

        public TypeLayout(NamedTypeMetaInfo type, IReadOnlyList<MemberMetaInfo> typeMembers) {
            Type = type;
            Layouts = typeMembers;
        }

        public override int GetHashCode() {
            var hash = new HashCode();
            hash.Add(Type);
            for (var i = 0; i < Layouts.Count; ++i)
                hash.Add(Layouts[i]);
            return hash.ToHashCode();
        }

        public override bool Equals(object obj) => obj is TypeLayout layout && Equals(layout);

        public bool Equals(TypeLayout layout) {
            if (layout.Layouts.Count != Layouts.Count)
                return false;

            if (!Type.Equals(layout.Type))
                return false;

            for (int i = 0; i < Layouts.Count; i++) {
                if (!Layouts[i].Equals(layout.Layouts[i]))
                    return false;
            }
            return true;
        }

    }
}
