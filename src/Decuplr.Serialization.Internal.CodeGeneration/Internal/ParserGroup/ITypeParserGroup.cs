using System.Collections.Generic;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal.ParserGroup {
    interface ITypeParserGroup {
        IReadOnlyList<string> PrependArguments { get; }
        ITypeSymbol TargetSymbol { get; }
        ParserMethodNames MethodNames { get; }
        void TryDeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult);
        void TryDeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor, OutArgs<object> outResult);
        void DeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes);
        void DeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor);
        void TrySerialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes);
        void Serialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan);
        void GetLength(CodeNodeBuilder node, InArgs<object> target);
    }
}
