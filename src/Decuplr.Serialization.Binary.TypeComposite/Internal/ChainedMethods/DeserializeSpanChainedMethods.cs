using System;
using System.Diagnostics;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal class DeserializeSpanChainedMethods : MemberChainedMethods<DeserializeSpanChainedMethods> {

        private static MethodSignature CreateMethod(TypeName memberName, MemberMetaInfo member, string methodName) {
            Debug.Assert(member.ReturnType is { });
            return MethodSignatureBuilder.CreateMethod(memberName, methodName)
                                         .AddArgument((RefKind.In, member.ContainingFullType.Symbol, "source"))
                                         .AddArgument((RefKind.Ref, TypeName.FromType(typeof(ReadOnlySpan<byte>)), "data"))
                                         .AddArgument((RefKind.Out, TypeName.FromType(member.ReturnType.Symbol), "result"))
                                         .WithReturn(TypeName.FromType<int>());
        }

        public DeserializeSpanChainedMethods(TypeName memberCompositeName, MemberMetaInfo member, Func<int?, string> nextMethodName)
            : base(i => CreateMethod(memberCompositeName, member, nextMethodName(i))) {
        }

        private DeserializeSpanChainedMethods(DeserializeSpanChainedMethods sourceMethods, bool hasNextMethod)
            : base(sourceMethods, hasNextMethod) {
        }

        protected override DeserializeSpanChainedMethods MoveNext(DeserializeSpanChainedMethods sourceMethod, bool hasNextMethod)
            => new DeserializeSpanChainedMethods(sourceMethod, hasNextMethod);
    }
}
