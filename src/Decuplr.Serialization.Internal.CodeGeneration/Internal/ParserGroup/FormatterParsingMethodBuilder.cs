using System;
using System.Collections.Generic;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.CodeGeneration.ParserGroup;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.Serialization.CodeGeneration.Internal.ParserGroup {
    internal class FormatterParsingMethodBuilder : ParsingMethodBuilder {
        private static class Method {
            public static string TryDeserializeState(int index) => $"TryDeserializeState_{index}";
            public static string DeserializeState(int index) => $"DeserializeState_{index}";
            public static string TrySerializeState(int index) => $"TrySerializeState_{index}";
            public static string SerializeState(int index) => $"SerializeState_{index}";
            public static string GetLengthState(int index) => $"GetLengthState_{index}";
        }


        private const string parent = "parent";

        private readonly IFormatterParsingMethod<TypeSourceArgs> _resolver;
        private readonly ParserMethodNames? _nextMethodNames;

        public override IReadOnlyList<string> PrependArguments { get; }

        public FormatterParsingMethodBuilder(IFormatterParsingMethod<TypeSourceArgs> resolver, int index, MemberMetaInfo member, bool shouldMoveNext) 
            : base(member, GetDefaultNames(index)) {
            _resolver = resolver;
            _nextMethodNames = shouldMoveNext ? GetDefaultNames(index + 1) : default;
            PrependArguments = new[] { $"in {member.ContainingFullType.Symbol} {parent}" };
        }

        private static ParserMethodNames GetDefaultNames(int index)
            => new ParserMethodNames {
                TryDeserializeSequence = Method.TryDeserializeState(index),
                TryDeserializeSpan = Method.TryDeserializeState(index),
                DeserializeSequence = Method.TryDeserializeState(index),
                DeserializeSpan = Method.TryDeserializeState(index),
                TrySerialize = Method.TrySerializeState(index),
                Serialize = Method.SerializeState(index),
            };

        public override void TryDeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor, OutArgs<object> outResult)
            => node.Add(_resolver.GetMethodBody(_nextMethodNames?.TryDeserializeSequence, new TryDeserializeSequenceArgs<TypeSourceArgs>(parent, refSequenceCursor, outResult)));

        public override void TryDeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult)
            => node.Add(_resolver.GetMethodBody(_nextMethodNames?.TryDeserializeSpan, new TryDeserializeSpanArgs<TypeSourceArgs>(parent, readOnlySpan, outReadBytes, outResult)));

        public override void DeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor)
            => node.Add(_resolver.GetMethodBody(_nextMethodNames?.TryDeserializeSequence, new DeserializeSequenceArgs<TypeSourceArgs>(parent, refSequenceCursor)));

        public override void DeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes)
            => node.Add(_resolver.GetMethodBody(_nextMethodNames?.TryDeserializeSpan, new DeserializeSpanArgs<TypeSourceArgs>(parent, readOnlySpan, outReadBytes)));

        public override void GetLength(CodeNodeBuilder node, InArgs<object> target)
             => node.Add(_resolver.GetMethodBody(_nextMethodNames?.GetLength, new GetLengthArgs<TypeSourceArgs>(parent, target)));

        public override void Serialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan)
            => node.Add(_resolver.GetMethodBody(_nextMethodNames?.Serialize, new SerializeArgs<TypeSourceArgs>(parent, target, readOnlySpan)));

        public override void TrySerialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes)
            => node.Add(_resolver.GetMethodBody(_nextMethodNames?.TrySerialize, new TrySerializeArgs<TypeSourceArgs>(parent, target, readOnlySpan, outWrittenBytes)));
    }
}
