using System.Collections.Generic;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal {
    internal readonly struct BinaryMemberCompositeStruct {
        public MemberMetaInfo CompositeMember { get; }
        public IReadOnlyList<MemberMetaInfo> RelyingMembers { get; }
        public IReadOnlyList<MethodSignature> EntryMethods { get; }
        public TypeName Name { get; }
        public int? ConstantLength { get; }

        public BinaryMemberCompositeStruct(TypeName name, MemberMetaInfo compositeMember, IReadOnlyList<MemberMetaInfo> relyingMembers, IReadOnlyList<MethodSignature> entryMethods, int? constantLength) {
            Name = name;
            CompositeMember = compositeMember;
            RelyingMembers = relyingMembers;
            EntryMethods = entryMethods;
            ConstantLength = constantLength;
        }
    }
}

