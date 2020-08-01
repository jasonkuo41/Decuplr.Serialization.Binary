using System;
using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal class ParsingBodyWrapBuilder : ParsingMethodBuilder {

        private readonly IComposerMethodBodyBuilder _body;

        public override IReadOnlyList<string> PrependArguments => Array.Empty<string>();

        public ParsingBodyWrapBuilder(MemberMetaInfo member, ComposerMethodNames methodNames, IComposerMethodBodyBuilder bodyBuilder)
            : base(member, methodNames) {
            _body = bodyBuilder;
        }

        public override void DeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor) => _body.DeserializeSequence(node, refSequenceCursor);
        public override void DeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes) => _body.DeserializeSpan(node, readOnlySpan, outReadBytes);
        public override void TryDeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor, OutArgs<object> outResult) => _body.TryDeserializeSequence(node, refSequenceCursor, outResult);
        public override void TryDeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult) => _body.TryDeserializeSpan(node, readOnlySpan, outReadBytes, outResult);
        public override void GetLength(CodeNodeBuilder node, InArgs<object> target) => _body.GetLength(node, target);
        public override void Serialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan) => _body.Serialize(node, target, readOnlySpan);
        public override void TrySerialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes) => _body.TrySerialize(node, target, readOnlySpan, outWrittenBytes);
    }
}
