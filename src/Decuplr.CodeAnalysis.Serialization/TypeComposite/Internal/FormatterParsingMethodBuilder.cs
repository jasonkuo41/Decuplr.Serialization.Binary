using System;
using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal class FormatterParsingMethodBuilder : ParsingMethodBuilder {

        private const string parent = "parent";

        private readonly IFormatterParsingMethod<TypeSourceArgs> _resolver;
        private readonly ComposerMethodNames? _nextMethodNames;

        public override IReadOnlyList<string> PrependArguments { get; }

        public FormatterParsingMethodBuilder(IFormatterParsingMethod<TypeSourceArgs> resolver, int index, MemberMetaInfo member, Func<int, ComposerMethodNames> defaultNames, bool shouldMoveNext)
            : base(member, defaultNames(index)) {
            _resolver = resolver;
            _nextMethodNames = shouldMoveNext ? defaultNames(index + 1) : default;
            PrependArguments = new[] { $"in {member.ContainingFullType.Symbol} {parent}" };
        }


        public override void TryDeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor, OutArgs<object> outResult)
            => node.AddPlain(_resolver.GetMethodBody(_nextMethodNames?.TryDeserializeSequence, new TryDeserializeSequenceArgs<TypeSourceArgs>(parent, refSequenceCursor, outResult)));

        public override void TryDeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult)
            => node.AddPlain(_resolver.GetMethodBody(_nextMethodNames?.TryDeserializeSpan, new TryDeserializeSpanArgs<TypeSourceArgs>(parent, readOnlySpan, outReadBytes, outResult)));

        public override void DeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor)
            => node.AddPlain(_resolver.GetMethodBody(_nextMethodNames?.TryDeserializeSequence, new DeserializeSequenceArgs<TypeSourceArgs>(parent, refSequenceCursor)));

        public override void DeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes)
            => node.AddPlain(_resolver.GetMethodBody(_nextMethodNames?.TryDeserializeSpan, new DeserializeSpanArgs<TypeSourceArgs>(parent, readOnlySpan, outReadBytes)));

        public override void GetLength(CodeNodeBuilder node, InArgs<object> target)
             => node.AddPlain(_resolver.GetMethodBody(_nextMethodNames?.GetLength, new GetLengthArgs<TypeSourceArgs>(parent, target)));

        public override void Serialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan)
            => node.AddPlain(_resolver.GetMethodBody(_nextMethodNames?.Serialize, new SerializeArgs<TypeSourceArgs>(parent, target, readOnlySpan)));

        public override void TrySerialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes)
            => node.AddPlain(_resolver.GetMethodBody(_nextMethodNames?.TrySerialize, new TrySerializeArgs<TypeSourceArgs>(parent, target, readOnlySpan, outWrittenBytes)));
    }
}
