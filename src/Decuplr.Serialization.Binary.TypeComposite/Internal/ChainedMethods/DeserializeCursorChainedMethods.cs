using System;
using System.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal class DeserializeCursorChainedMethods : MemberChainedMethods<DeserializeCursorChainedMethods> {
        public DeserializeCursorChainedMethods(TypeName memberCompositeName, MemberMetaInfo member, Func<int?, string> nextMethodName)
            : base(i => CreateMethod(memberCompositeName, member, nextMethodName(i))) {
        }

        private DeserializeCursorChainedMethods(DeserializeCursorChainedMethods sourceMethods, bool hasNextMethod)
            : base(sourceMethods, hasNextMethod) {
        }

        private static MethodSignature CreateMethod(TypeName memberName, MemberMetaInfo member, string methodName) {
            Debug.Assert(member.ReturnType is { });
            return MethodSignatureBuilder.CreateMethod(memberName, methodName)
                                         .AddArgument((RefKind.In, member.ContainingFullType.Symbol, "source"))
                                         .AddArgument((RefKind.Ref, TypeName.FromType(typeof(SequenceCursor<byte>)), "cursor"))
                                         .AddArgument((RefKind.Out, member.ReturnType.Symbol, "result"))
                                         .WithReturn(TypeName.FromType<bool>());
        }

        protected override DeserializeCursorChainedMethods MoveNext(DeserializeCursorChainedMethods sourceMethod, bool hasNextMethod)
            => new DeserializeCursorChainedMethods(sourceMethod, hasNextMethod);
    }
}
