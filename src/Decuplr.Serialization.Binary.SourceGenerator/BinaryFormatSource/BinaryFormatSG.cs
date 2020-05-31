using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.Analyzers;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator.BinaryFormatSource {
    internal class BinaryFormatSG : IParserGenerateSource {
        public GeneratedTypeParser GenerateParser(IEnumerable<AnalyzedType> types, SourceGeneratorContext context) {
            var interestedType = types.Where(type => type.ContainsAttribute<BinaryFormatAttribute>()).Select(type => CreateParser(type));

        }

        public bool TryCreateParser(AnalyzedType type, SourceGeneratorContext context, out GeneratedTypeParser parser) {
            // First we check if the type's layout and dump the output of the analyzed format
            parser = default;
            var result = TypeFormatLayout.TryGetLayout(type, out var diagnostics, out var typeFormatLayout);
            foreach (var diagnostic in diagnostics)
                context.ReportDiagnostic(diagnostic);
            if (!result)
                return false;

            // Check if the target parser would need to use `partial` keyword to help up invoke into the code
            // The coditionals are simple, if we can all the access to index members, then we just need an observer.

        }
    }

}
