using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {

    internal class GetBlockLengthSpanChainedMethods : BlockLengthChainedMethods<GetBlockLengthSpanChainedMethods> {
        public GetBlockLengthSpanChainedMethods(TypeName memberCompositeName, MemberMetaInfo member, IEnumerable<MemberMetaInfo> relyingMembers, Func<int?, string> nextMethodName)
            : base(i => CreateMethod(memberCompositeName, member, relyingMembers, nextMethodName(i))) {
        }

        private GetBlockLengthSpanChainedMethods(GetBlockLengthSpanChainedMethods sourceMethods, bool hasNextMethod)
            : base(sourceMethods, hasNextMethod) {
        }

        private static MethodSignature CreateMethod(TypeName memberName, MemberMetaInfo member, IEnumerable<MemberMetaInfo> relyingMembers, string methodName) {
            Debug.Assert(member.ReturnType is { });
            return MethodSignatureBuilder.CreateMethod(memberName, methodName)
                                         .AddArgument((TypeName.FromType(typeof(ReadOnlySpan<byte>)), "data"))
                                         .AddArgument(GetRelyingArgs(relyingMembers))
                                         .WithReturn(TypeName.FromType<int>());
        }

        protected override GetBlockLengthSpanChainedMethods MoveNext(GetBlockLengthSpanChainedMethods sourceMethod, bool hasNextMethod)
            => new GetBlockLengthSpanChainedMethods(sourceMethod, hasNextMethod);
    }
}
