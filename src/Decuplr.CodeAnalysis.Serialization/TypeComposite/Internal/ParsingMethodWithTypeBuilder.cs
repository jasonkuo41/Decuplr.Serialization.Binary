using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal class ParsingMethodWithTypeBuilder : ParsingMethodBuilder {

        private const string parent = "parent";

        public override IReadOnlyList<string> PrependArguments { get; }

        public ParsingMethodWithTypeBuilder(MemberMetaInfo member)
            : base(member, ComposerMethodNames.DefaultNames) {
            PrependArguments = new[] { $"in {member.ContainingFullType.Symbol} {parent}" };
        }

        public override void DeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor)
            => node.AddPlain($"{MemberMethod.TryDeserializeState(0)}({parent}, ref {refSequenceCursor})");

        public override void DeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes)
            => node.Return($"{MemberMethod.TryDeserializeState(0)}({parent}, {readOnlySpan}, out {outReadBytes})");

        public override void GetLength(CodeNodeBuilder node, InArgs<object> target)
            => node.Return($"{MemberMethod.GetLengthState(0)}({parent}, {target} )");

        public override void Serialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan)
            => node.Return($"{MemberMethod.SerializeState(0)}({parent}, {target}, {readOnlySpan})");

        public override void TryDeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor, OutArgs<object> outResult)
            => node.Return($"{MemberMethod.TryDeserializeState(0)}({parent}, ref {refSequenceCursor}, out {outResult})");

        public override void TryDeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult)
            => node.Return($"{MemberMethod.TryDeserializeState(0)}({parent}, {readOnlySpan}, out {outReadBytes}, out {outResult})");

        public override void TrySerialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes)
            => node.Return($"{MemberMethod.TrySerializeState(0)} ({parent}, {target}, {readOnlySpan}, out {outWrittenBytes})");
    }
}
