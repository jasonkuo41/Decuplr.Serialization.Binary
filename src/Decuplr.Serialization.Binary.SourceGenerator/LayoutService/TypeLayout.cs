using System.Collections.Generic;
using Decuplr.Serialization.Binary.AnalysisService;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal class TypeLayout {
        public TypeMetaInfo Type { get; }
        public IReadOnlyList<MemberMetaInfo> Layouts { get; }

        public TypeLayout(TypeMetaInfo type, IReadOnlyList<MemberMetaInfo> typeMembers) {
            Type = type;
            Layouts = typeMembers;
        }
    }
}
