using System;
using System.Buffers;
using System.Diagnostics;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal sealed class SerializeWriterChainedMethods : WriteStateChainedMethods<SerializeWriterChainedMethods>, IChainedMethods {

        public const string T_WRITER = "TWriter";

        public static MethodSignature CreateMethodSignature(TypeName memberCompositeName, MemberMetaInfo member, string methodName) {
            Debug.Assert(member.ReturnType != null);
            return MethodSignatureBuilder.CreateMethod(memberCompositeName, methodName)
                                         .AddGenerics(T_STATE, GenericConstrainKind.Struct, new TypeName("Decuplr.Serialization.Binary", "IBinaryWriteState<TState>"))
                                         .AddGenerics(T_WRITER, GenericConstrainKind.Struct, TypeName.FromType<IBufferWriter<byte>>())
                                         .AddArgument((RefKind.In, member.ReturnType.Symbol, "member"))
                                         .AddArgument((RefKind.In, member.ContainingFullType.Symbol, "source"))
                                         .AddArgument((TypeName.FromGenericArgument(T_STATE), "state"))
                                         .AddArgument((RefKind.Ref, TypeName.FromGenericArgument(T_WRITER), "writer"))
                                         .WithReturn(TypeName.Void);
        }

        public SerializeWriterChainedMethods(TypeName memberCompositeName, MemberMetaInfo member, Func<int?, string> nextMethodName)
                : base(id => CreateMethodSignature(memberCompositeName, member, nextMethodName(id))) {
        }

        private SerializeWriterChainedMethods(SerializeWriterChainedMethods sourceMethod, bool hasNextMethod)
            : base(sourceMethod, hasNextMethod) { }

        protected override SerializeWriterChainedMethods MoveNext(SerializeWriterChainedMethods sourceMethod, bool hasNextMethod)
            => new SerializeWriterChainedMethods(sourceMethod, hasNextMethod);
    }
}
