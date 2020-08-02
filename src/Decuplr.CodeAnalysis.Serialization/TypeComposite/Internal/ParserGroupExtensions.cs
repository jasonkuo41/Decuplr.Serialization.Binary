using System;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
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
