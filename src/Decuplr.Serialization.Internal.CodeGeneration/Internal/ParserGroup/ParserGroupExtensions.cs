using System;
using System.Text;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.CodeGeneration.ParserGroup;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.Serialization.CodeGeneration.Internal.ParserGroup {
    internal static class ParserGroupExtensions {
        public static CodeNodeBuilder AddParsingMethods(this CodeNodeBuilder builder, ParsingMethodBuilder group) => group.Build(builder);

        public static CodeNodeBuilder AddFormatterFinalMethods(this CodeNodeBuilder builder, IFormatterParsingMethod<TypeSourceArgs> resolver, MemberMetaInfo member, int index, Func<int, ParserMethodNames> defaultNames)
           => builder.AddParsingMethods(new FormatterParsingMethodBuilder(resolver, index, member, defaultNames, false));

        public static CodeNodeBuilder AddFormatterMethods(this CodeNodeBuilder builder, IFormatterParsingMethod<TypeSourceArgs> resolver, MemberMetaInfo member, int index, Func<int, ParserMethodNames> defaultNames)
           => builder.AddParsingMethods(new FormatterParsingMethodBuilder(resolver, index, member, defaultNames, true));

        public static CodeNodeBuilder AddParsingBody(this CodeNodeBuilder builder, IParsingMethodBody bodyBuilder, MemberMetaInfo member, ParserMethodNames names)
            => builder.AddParsingMethods(new ParsingBodyWrapBuilder(member, names, bodyBuilder));

    }
}
