using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator.ParserSource {
    internal class BinaryParserSG : IParserGenerateSource {
        public GeneratedTypeParser GenerateParser(IEnumerable<AnalyzedType> types, SourceGeneratorContext context) {
            throw new NotImplementedException();
        }
    }
}
