using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.CodeGeneration.ParserGroup;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal.ParserGroup {
    internal abstract class ParsingMethodBuilder : IParsingMethodBody {

        private readonly Accessibility _accessibility;

        public ParserMethodNames MethodNames { get; }

        public virtual IReadOnlyList<string> PrependArguments { get; } = Array.Empty<string>();

        public ITypeSymbol TargetSymbol { get; }

        public ParsingMethodBuilder(MemberMetaInfo member, ParserMethodNames methodNames) 
            : this (member, methodNames, Accessibility.Private) {
        }

        public ParsingMethodBuilder(MemberMetaInfo member, ParserMethodNames methodNames, Accessibility accessibility) {
            if (member.ReturnType is null)
                throw new ArgumentException("Member without return type cannot be configured for parser groups");
            TargetSymbol = member.ReturnType.Symbol;
            MethodNames = methodNames;
            _accessibility = accessibility;
        }

        public abstract void TryDeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor, OutArgs<object> outResult);
        public abstract void TryDeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult);
        public abstract void DeserializeSequence(CodeNodeBuilder node, BufferArgs refSequenceCursor);
        public abstract void DeserializeSpan(CodeNodeBuilder node, BufferArgs readOnlySpan, OutArgs<int> outReadBytes);
        public abstract void GetLength(CodeNodeBuilder node, InArgs<object> target);
        public abstract void Serialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan);
        public abstract void TrySerialize(CodeNodeBuilder node, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes);

        public CodeNodeBuilder Build(CodeNodeBuilder builder) {

            const string ReadOnlySpanByte = "ReadOnlySpan<byte>";
            const string SpanByte = "Span<byte>";
            const string SequenceCursor = "SequenceCursor<byte>";

            const string span = "span";
            const string cursor = "cursor";
            const string readBytes = "readBytes";
            const string writtenBytes = "writtenBytes";
            const string target = "target";

            var accessMod = _accessibility.ToString().ToLower();

            var strBuilder = new StringBuilder();
            foreach (var arg in PrependArguments) {
                strBuilder.Append(arg);
                strBuilder.Append(", ");
            }
            var prependString = strBuilder.ToString();

            // TryDeserialize (ReadOnlySpan<byte>)
            builder.AddNode($"{accessMod} {nameof(DeserializeResult)} {MethodNames.TryDeserializeSpan}({prependString}{ReadOnlySpanByte} {span}, out int {readBytes}, out {TargetSymbol} {target})",
                node => TryDeserializeSpan(node, span, readBytes, target));

            // TryDeserialize (Sequence)
            builder.AddNode($"{accessMod} {nameof(DeserializeResult)} {MethodNames.TryDeserializeSequence}({prependString}{SequenceCursor} {cursor}, out {TargetSymbol} {target})",
                node => TryDeserializeSequence(node, cursor, target));

            // Deserialize (ReadOnlySpan<byte>)
            builder.AddNode($"{accessMod} {TargetSymbol} {MethodNames.DeserializeSpan}({prependString}{ReadOnlySpanByte} {span}, out int {readBytes})",
                node => DeserializeSpan(node, span, readBytes));

            // Deserialize (Sequence)
            builder.AddNode($"{accessMod} int {MethodNames.DeserializeSequence}({prependString}{SequenceCursor} {cursor})",
                node => DeserializeSequence(node, cursor));


            // TrySerialize (ReadOnlySpan<byte>)
            builder.AddNode($"{accessMod} bool {MethodNames.TrySerialize}({prependString}in {TargetSymbol} {target}, {SpanByte} {span}, out int {writtenBytes})",
                node => TrySerialize(node, target, span, writtenBytes));

            // Serialize (ReadOnlySpan<byte>)
            builder.AddNode($"{accessMod} int {MethodNames.Serialize}({prependString}in {TargetSymbol} {target}, {SpanByte} {span})",
                node => Serialize(node, target, span));

            // GetLength
            builder.AddNode($"{accessMod} int {MethodNames.GetLength}({prependString}in {TargetSymbol} {target})",
                node => GetLength(node, target));

            return builder;
        }
    }
}
