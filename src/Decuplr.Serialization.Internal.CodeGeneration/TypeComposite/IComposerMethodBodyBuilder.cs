using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.Serialization.CodeGeneration.TypeComposite {
    public interface IComposerMethodBodyBuilder {
        void TryDeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult);
        void TryDeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor, OutArgs<object> outResult);
        void DeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes);
        void DeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor);
        void TrySerialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes);
        void Serialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan);
        void GetLength(CodeNodeBuilder node, InArgs<object> target);
    }
}
