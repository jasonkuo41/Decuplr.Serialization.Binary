using System.Text;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.Serialization.CodeGeneration.Internal.ParserGroup {
    internal static class ParserGroupExtensions {
        public static CodeNodeBuilder AddTypeParserGroup(this CodeNodeBuilder builder, ITypeParserGroup group) {
            const string ReadOnlySpanByte = "ReadOnlySpan<byte>";
            const string SpanByte = "Span<byte>";
            const string SequenceCursor = "SequenceCursor<byte>";

            const string span = "span";
            const string cursor = "cursor";
            const string readBytes = "readBytes";
            const string writtenBytes = "writtenBytes";
            const string target = "target";

            var strBuilder = new StringBuilder();
            foreach (var arg in group.PrependArguments) {
                strBuilder.Append(arg);
                strBuilder.Append(", ");
            }
            var prependString = strBuilder.ToString();

            // TryDeserialize (ReadOnlySpan<byte>)
            builder.AddNode($"private {nameof(DeserializeResult)} {group.MethodNames.TryDeserializeSpan}({prependString}{ReadOnlySpanByte} {span}, out int {readBytes}, out {group.TargetSymbol} {target})",
                node => group.TryDeserializeSpan(node, span, readBytes, target));

            // TryDeserialize (Sequence)
            builder.AddNode($"private {nameof(DeserializeResult)} {group.MethodNames.TryDeserializeSequence}({prependString}{SequenceCursor} {cursor}, out {group.TargetSymbol} {target})",
                node => group.TryDeserializeSequence(node, cursor, target));

            // Deserialize (ReadOnlySpan<byte>)
            builder.AddNode($"private {group.TargetSymbol} {group.MethodNames.DeserializeSpan}({prependString}{ReadOnlySpanByte} {span}, out int {readBytes})",
                node => group.DeserializeSpan(node, span, readBytes));

            // Deserialize (Sequence)
            builder.AddNode($"private int {group.MethodNames.DeserializeSequence}({prependString}{SequenceCursor} {cursor})",
                node => group.DeserializeSequence(node, cursor));


            // TrySerialize (ReadOnlySpan<byte>)
            builder.AddNode($"private bool {group.MethodNames.TrySerialize}({prependString}in {group.TargetSymbol} {target}, {SpanByte} {span}, out int {writtenBytes})",
                node => group.TrySerialize(node, target, span, writtenBytes));

            // Serialize (ReadOnlySpan<byte>)
            builder.AddNode($"private int {group.MethodNames.Serialize}({prependString}in {group.TargetSymbol} {target}, {SpanByte} {span})",
                node => group.Serialize(node, target, span));

            // GetLength
            builder.AddNode($"private int {group.MethodNames.GetLength}({prependString}in {group.TargetSymbol} {target})",
                node => group.GetLength(node, target));

            return builder;
        }

        public static CodeNodeBuilder AddFormatterParserGroup(this CodeNodeBuilder builder, IFormmaterBase<TypeSourceArgs> resolver, MemberMetaInfo member, int index)
           => builder.AddTypeParserGroup(new ResolverStateParserGroup(resolver, member, index));
    }
}
