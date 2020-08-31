using System;
using System.Diagnostics;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal class GetSpanLengthChainedMethods : WriteStateChainedMethods<GetSpanLengthChainedMethods> {
        public GetSpanLengthChainedMethods(TypeName memberCompositeName, MemberMetaInfo member, Func<int?, string> nextMethodName)
            : base(i => CreateMethod(memberCompositeName, member, nextMethodName(i))) {
        }

        public GetSpanLengthChainedMethods(GetSpanLengthChainedMethods sourceMethods, bool hasNextMethod)
            : base(sourceMethods, hasNextMethod) {
        }

        private static MethodSignature CreateMethod(TypeName memberName, MemberMetaInfo member, string methodName) {
            Debug.Assert(member.ReturnType is { });
            return MethodSignatureBuilder.CreateMethod(memberName, methodName)
                                         .AddGenerics(T_STATE, GenericConstrainKind.Struct, new TypeName("Decuplr.Serialization.Binary", "IBinaryWriteState<TState>"))
                                         .AddArgument((RefKind.In, member.ReturnType.Symbol, "member"))
                                         .AddArgument((RefKind.In, member.ContainingFullType.Symbol, "source"))
                                         .AddArgument((TypeName.FromGenericArgument(T_STATE), "state"))
                                         .WithReturn(TypeName.FromType<int>());
        }

        protected override GetSpanLengthChainedMethods MoveNext(GetSpanLengthChainedMethods sourceMethod, bool hasNextMethod)
            => new GetSpanLengthChainedMethods(sourceMethod, hasNextMethod);
    }
}
