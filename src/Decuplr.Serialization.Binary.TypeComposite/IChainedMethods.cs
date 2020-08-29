using System;
using System.Collections.Generic;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public interface IChainedMethods {
        bool HasChainedMethod { get; }

        IReadOnlyList<MethodTypeParams> TypeParameters { get; }
        IReadOnlyList<MethodArg> Arguments { get; }

        MethodArg this[TypeName typeName] { get; }
        MethodArg this[TypeName typeName, int index] { get; }

        string InvokeNextMethod();
        string InvokeNextMethod(Action<IChainedMethodsModifier> action);
    }
}
