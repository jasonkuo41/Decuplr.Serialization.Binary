using System.Collections.Generic;
using Decuplr.Serialization.AnalysisService;

namespace Decuplr.Serialization.LayoutService {
    public class TypeLayout {
        public NamedTypeMetaInfo Type { get; }
        public IReadOnlyList<MemberMetaInfo> Layouts { get; }

        public TypeLayout(NamedTypeMetaInfo type, IReadOnlyList<MemberMetaInfo> typeMembers) {
            Type = type;
            Layouts = typeMembers;
        }
    }
}
