using System.Collections.Generic;
using Decuplr.Serialization.Binary.AnalysisService;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal class TypeLayout {
        public NamedTypeMetaInfo Type { get; }
        public IReadOnlyList<MemberMetaInfo> Layouts { get; }

        public TypeLayout(NamedTypeMetaInfo type, IReadOnlyList<MemberMetaInfo> typeMembers) {
            Type = type;
            Layouts = typeMembers;
        }
    }
}
