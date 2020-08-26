using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.TypeComposite {
    internal interface IChainedMethods {
        bool HasChainedMethod { get; }

        string this[TypeName type] { get; }
        string this[TypeName type, int index] { get; }

        string InvokeNextMethod();
        string InvokeNextMethod(Action<IChainMethodInvokeAction> action);
    }

    internal interface IChainMethodInvokeAction {
        string this[TypeName type] { get; set; }
        string this[TypeName type, int index] { get; set; }
    }

    internal interface IBinaryMemberComposeMethod {
        void SerializeWriter(CodeNodeBuilder builder, IChainedMethods chained);
        void SerializeSpan(CodeNodeBuilder builder, IChainedMethods chained);

        void GetSpanLength(CodeNodeBuilder builder, IChainedMethods chained);

        void DeserializeSpan(CodeNodeBuilder builder, IChainedMethods chained);
        void DeserializeCursor(CodeNodeBuilder builder, IChainedMethods chained);

        void GetBlockLengthSpan(CodeNodeBuilder builder, IChainedMethods chained, );
        void GetBlockLengthCursor(CodeNodeBuilder builder, IChainedMethods chained, );
    }
}
