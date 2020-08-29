using System;
using System.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal class GetBlockLengthSpanChainedMethods : MemberChainedMethods<GetBlockLengthSpanChainedMethods> {
        public GetBlockLengthSpanChainedMethods(TypeName memberCompositeName, MemberMetaInfo member, Func<int?, string> nextMethodName)
            : base(i => CreateMethod(memberCompositeName, member, nextMethodName(i))) {
        }

        private GetBlockLengthSpanChainedMethods(GetBlockLengthSpanChainedMethods sourceMethods, bool hasNextMethod)
            : base(sourceMethods, hasNextMethod) {
        }

        private static MethodSignature CreateMethod(TypeName memberName, MemberMetaInfo member, string methodName) {
            Debug.Assert(member.ReturnType is { });
            return MethodSignatureBuilder.CreateMethod(memberName, methodName)
                                         .AddArgument((TypeName.FromType(typeof(ReadOnlySpan<byte>)), "data"))
                                         .WithReturn(TypeName.FromType<int>());
        }

        protected override GetBlockLengthSpanChainedMethods MoveNext(GetBlockLengthSpanChainedMethods sourceMethod, bool hasNextMethod)
            => new GetBlockLengthSpanChainedMethods(sourceMethod, hasNextMethod);
    }
}
