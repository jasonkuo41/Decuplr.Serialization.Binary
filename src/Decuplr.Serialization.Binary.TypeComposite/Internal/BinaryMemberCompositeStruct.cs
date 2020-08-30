using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal {
    internal readonly struct BinaryMemberCompositeStruct {
        public MemberMetaInfo CompositeMember { get; }
        public IReadOnlyList<MemberMetaInfo> RelyingMembers { get; }
        public IReadOnlyList<MethodSignature> EntryMethods { get; }
        public int? ConstantLength { get; }

        public BinaryMemberCompositeStruct(MemberMetaInfo compositeMember, IReadOnlyList<MemberMetaInfo> relyingMembers, IReadOnlyList<MethodSignature> entryMethods, int? constantLength) {
            CompositeMember = compositeMember;
            RelyingMembers = relyingMembers;
            EntryMethods = entryMethods;
            ConstantLength = constantLength;
        }
    }
}

