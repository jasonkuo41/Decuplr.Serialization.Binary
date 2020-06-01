using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    interface IParserGenerateSource {
        GeneratedTypeParser GenerateParser(IEnumerable<AnalyzedType> types, SourceGeneratorContext context);
    }

    internal struct GeneratedTypeParser {
        public IReadOnlyList<GeneratedSourceCode> AdditionalFiles { get; set; }
        public IReadOnlyList<GeneratedParser> GeneratedParser { get; set; }
        public IReadOnlyList<string> AdditionalClasses { get; set; }
    }
}
