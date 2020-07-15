using System.Text;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.CodeGeneration.ParserGroup;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.Serialization.CodeGeneration.Internal.ParserGroup {
    internal static class ParserGroupExtensions {
        public static CodeNodeBuilder AddParsingMethods(this CodeNodeBuilder builder, ParsingMethodBuilder group) => group.Build(builder);

        public static CodeNodeBuilder AddFormatterParsingMethods(this CodeNodeBuilder builder, IFormatterParsingMethod<TypeSourceArgs> resolver, MemberMetaInfo member, int index, bool shouldMoveNext)
           => builder.AddParsingMethods(new FormatterParsingMethodBuilder(resolver, index, member, shouldMoveNext));

        public static CodeNodeBuilder AddParsingBody(this CodeNodeBuilder builder, IParsingMethodBody bodyBuilder, MemberMetaInfo member, ParserMethodNames names)
            => builder.AddParsingMethods(new ParsingBodyWrapBuilder(member, names, bodyBuilder));
    }
}
