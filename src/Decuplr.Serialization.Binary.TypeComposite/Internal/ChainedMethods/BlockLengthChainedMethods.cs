using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal abstract class BlockLengthChainedMethods<T> : MemberChainedMethods<T> where T : BlockLengthChainedMethods<T> {
        protected BlockLengthChainedMethods(Func<int?, MethodSignature> nextMethodSignature) 
            : base(nextMethodSignature) {
        }

        protected BlockLengthChainedMethods(T sourceMethods, bool hasNextMethod) 
            : base(sourceMethods, hasNextMethod) {
        }

        protected static IEnumerable<MethodArg> GetRelyingArgs(IEnumerable<MemberMetaInfo> relyingMembers) => relyingMembers.Select((member, i) => new MethodArg(RefKind.In, TypeName.FromType(member.ReturnType.Symbol), $"args{i}"));

    }
}
