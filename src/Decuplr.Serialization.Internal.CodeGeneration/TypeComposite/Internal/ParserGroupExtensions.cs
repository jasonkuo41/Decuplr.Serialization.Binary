using System;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.Serialization.CodeGeneration.TypeComposite.Internal {
    internal static class ParserGroupExtensions {
        public static CodeNodeBuilder AddParsingMethods(this CodeNodeBuilder builder, ParsingMethodBuilder group) => group.Build(builder);

        public static CodeNodeBuilder AddFormatterFinalMethods(this CodeNodeBuilder builder, IFormatterParsingMethod<TypeSourceArgs> resolver, MemberMetaInfo member, int index, Func<int, ComposerMethodNames> defaultNames)
           => builder.AddParsingMethods(new FormatterParsingMethodBuilder(resolver, index, member, defaultNames, false));

        public static CodeNodeBuilder AddFormatterMethods(this CodeNodeBuilder builder, IFormatterParsingMethod<TypeSourceArgs> resolver, MemberMetaInfo member, int index, Func<int, ComposerMethodNames> defaultNames)
           => builder.AddParsingMethods(new FormatterParsingMethodBuilder(resolver, index, member, defaultNames, true));

        public static CodeNodeBuilder AddParsingBody(this CodeNodeBuilder builder, IComposerMethodBodyBuilder bodyBuilder, MemberMetaInfo member, ComposerMethodNames names)
            => builder.AddParsingMethods(new ParsingBodyWrapBuilder(member, names, bodyBuilder));

    }
}
