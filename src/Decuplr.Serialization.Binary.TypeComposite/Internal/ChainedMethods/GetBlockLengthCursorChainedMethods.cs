using System;
using System.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal class GetBlockLengthCursorChainedMethods : MemberChainedMethods<GetBlockLengthCursorChainedMethods> {
        public GetBlockLengthCursorChainedMethods(TypeName memberCompositeName, MemberMetaInfo member, Func<int?, string> nextMethodName)
            : base(i => CreateMethod(memberCompositeName, member, nextMethodName(i))) {
        }

        private GetBlockLengthCursorChainedMethods(GetBlockLengthCursorChainedMethods sourceMethods, bool hasNextMethod)
            : base(sourceMethods, hasNextMethod) {
        }

        private static MethodSignature CreateMethod(TypeName memberName, MemberMetaInfo member, string methodName) {
            Debug.Assert(member.ReturnType is { });
            return MethodSignatureBuilder.CreateMethod(memberName, methodName)
                                         .AddArgument((RefKind.Ref, TypeName.FromType(typeof(SequenceCursor<byte>)), "cursor"))
                                         .WithReturn(TypeName.FromType<int>());
        }

        protected override GetBlockLengthCursorChainedMethods MoveNext(GetBlockLengthCursorChainedMethods sourceMethod, bool hasNextMethod)
            => new GetBlockLengthCursorChainedMethods(sourceMethod, hasNextMethod);
    }
}
