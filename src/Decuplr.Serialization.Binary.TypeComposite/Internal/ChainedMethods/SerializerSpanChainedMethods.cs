using System;
using System.Diagnostics;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal sealed class SerializerSpanChainedMethods : WriteStateChainedMethods<SerializerSpanChainedMethods>, IChainedMethods {

        public static MethodSignature CreateMethodSignature(TypeName memberCompositeName, MemberMetaInfo member, string methodName) {
            Debug.Assert(member.ReturnType != null);
            return MethodSignatureBuilder.CreateMethod(memberCompositeName, methodName)
                                         .AddGenerics(T_STATE, GenericConstrainKind.Struct, new TypeName("Decuplr.Serialization.Binary", "IBinaryWriteState<TState>"))
                                         .AddArgument((RefKind.In, member.ReturnType.Symbol, "member"))
                                         .AddArgument((RefKind.In, member.ContainingFullType.Symbol, "source"))
                                         .AddArgument((TypeName.FromGenericArgument(T_STATE), "state"))
                                         .AddArgument((TypeName.FromType(typeof(Span<byte>)), "data"))
                                         .WithReturn(TypeName.FromType<int>());
        }

        public SerializerSpanChainedMethods(TypeName memberCompositeName, MemberMetaInfo member, Func<int?, string> nextMethodName)
            : base(id => CreateMethodSignature(memberCompositeName, member, nextMethodName(id))) {
        }

        private SerializerSpanChainedMethods(SerializerSpanChainedMethods sourceMethods, bool hasNextMethod) 
            : base(sourceMethods, hasNextMethod) {
        }

        protected override SerializerSpanChainedMethods MoveNext(SerializerSpanChainedMethods sourceMethod, bool hasNextMethod)
            => new SerializerSpanChainedMethods(sourceMethod, hasNextMethod);
    }
}
