using System;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal abstract class WriteStateChainedMethods<T> : MemberChainedMethods<T> where T : WriteStateChainedMethods<T> {

        public const string T_STATE = "TState";

        protected WriteStateChainedMethods(Func<int?, MethodSignature> nextMethodSignature) 
            : base(nextMethodSignature) {
        }

        protected WriteStateChainedMethods(T sourceMethods, bool hasNextMethod) 
            : base(sourceMethods, hasNextMethod) {
        }

    }
}
