using System;
using System.Collections.Generic;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.CodeGeneration.ParserGroup;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal.ParserGroup {
    internal abstract class StateParserGroup : ITypeParserGroup {
        private static class Method {
            public static string TryDeserializeState(int index) => $"TryDeserializeState_{index}";
            public static string DeserializeState(int index) => $"DeserializeState_{index}";
            public static string TrySerializeState(int index) => $"TrySerializeState_{index}";
            public static string SerializeState(int index) => $"SerializeState_{index}";
            public static string GetLengthState(int index) => $"GetLengthState_{index}";
        }

        private const string parent = "parent";

        public ParserMethodNames MethodNames { get; }

        public IReadOnlyList<string> PrependArguments { get; }

        public ITypeSymbol TargetSymbol { get; }

        public StateParserGroup(MemberMetaInfo member, int index) 
            : this(member, GetDefaultNames(index)) {
        }

        public StateParserGroup(MemberMetaInfo member, ParserMethodNames methodNames) {
            if (member.ReturnType is null)
                throw new ArgumentException("Member without return type cannot be configured for parser groups");
            TargetSymbol = member.ReturnType.Symbol;
            PrependArguments = new[] { $"in {member.ContainingFullType.Symbol} {parent}" };
            MethodNames = methodNames;
        }

        protected static ParserMethodNames GetDefaultNames(int index)
            => new ParserMethodNames {
                TryDeserializeSequence = Method.TryDeserializeState(index),
                TryDeserializeSpan = Method.TryDeserializeState(index),
                DeserializeSequence = Method.TryDeserializeState(index),
                DeserializeSpan = Method.TryDeserializeState(index),
                TrySerialize = Method.TrySerializeState(index),
                Serialize = Method.SerializeState(index),
            };

        protected abstract void TryDeserializeSpan(CodeNodeBuilder node, TryDeserializeSpanArgs<TypeSourceArgs> args);
        protected abstract void TryDeserializeSequence(CodeNodeBuilder node, TryDeserializeSequenceArgs<TypeSourceArgs> args);
        protected abstract void DeserializeSpan(CodeNodeBuilder node, DeserializeSpanArgs<TypeSourceArgs> args);
        protected abstract void DeserializeSequence(CodeNodeBuilder node, DeserializeSequenceArgs<TypeSourceArgs> args);
        protected abstract void TrySerialize(CodeNodeBuilder node, TrySerializeArgs<TypeSourceArgs> args);
        protected abstract void Serialize(CodeNodeBuilder node, SerializeArgs<TypeSourceArgs> args);
        protected abstract void GetLength(CodeNodeBuilder node, GetLengthArgs<TypeSourceArgs> args);

        public void TryDeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor, OutArgs<object> outResult)
            => TryDeserializeSequence(node, new TryDeserializeSequenceArgs<TypeSourceArgs>(parent, refSequenceCursor, outResult));

        public void TryDeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult)
            => TryDeserializeSpan(node, new TryDeserializeSpanArgs<TypeSourceArgs>(parent, readOnlySpan, outReadBytes, outResult));

        public void DeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor)
            => DeserializeSequence(node, new DeserializeSequenceArgs<TypeSourceArgs>(parent, refSequenceCursor));

        public void DeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes)
            => DeserializeSpan(node, new DeserializeSpanArgs<TypeSourceArgs>(parent, readOnlySpan, outReadBytes));

        public void GetLength(CodeNodeBuilder node, InArgs<object> target)
            => GetLength(node, new GetLengthArgs<TypeSourceArgs>(parent, target));

        public void Serialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan)
            => Serialize(node, new SerializeArgs<TypeSourceArgs>(parent, target, readOnlySpan));

        public void TrySerialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes)
            => TrySerialize(node, new TrySerializeArgs<TypeSourceArgs>(parent, target, readOnlySpan, outWrittenBytes));
    }
}
