using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.TypeComposite {

    public interface IBinaryMemberComposeMethod {
        void SerializeWriter(CodeNodeBuilder builder, IChainedMethods chained);
        void SerializeSpan(CodeNodeBuilder builder, IChainedMethods chained);

        void GetSpanLength(CodeNodeBuilder builder, IChainedMethods chained);

        void DeserializeSpan(CodeNodeBuilder builder, IChainedMethods chained);
        void DeserializeCursor(CodeNodeBuilder builder, IChainedMethods chained);

        void GetBlockLengthSpan(CodeNodeBuilder builder, IChainedMethods chained);
        void GetBlockLengthCursor(CodeNodeBuilder builder, IChainedMethods chained);
    }
}
