using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.CodeAnalysis.Diagnostics {
    public class TypeMetaSelection {
        public IReadOnlyList<MemberMetaInfo> SelectedMembers { get; }
        public IReadOnlyList<MemberMetaInfo> UnselectedMembers { get; }
        public NamedTypeMetaInfo Type { get; }

        public TypeMetaSelection(NamedTypeMetaInfo type, IReadOnlyList<MemberMetaInfo> selectedMember) {
            var selectedHash = new HashSet<MemberMetaInfo>(selectedMember);
            Type = type;
            SelectedMembers = selectedMember;
            UnselectedMembers = type.Members.Where(x => !selectedHash.Contains(x)).ToList();
        }

        private TypeMetaSelection(NamedTypeMetaInfo type, IReadOnlyList<MemberMetaInfo> selectedMembers, IReadOnlyList<MemberMetaInfo> unselectedMembers) {
            SelectedMembers = selectedMembers;
            UnselectedMembers = unselectedMembers;
            Type = type;
        }

        public static TypeMetaSelection Any(NamedTypeMetaInfo type) => new TypeMetaSelection(type, type.Members, Array.Empty<MemberMetaInfo>());
        public TypeMetaSelection ReverseSelection() => new TypeMetaSelection(Type, UnselectedMembers, SelectedMembers);
    }

}
