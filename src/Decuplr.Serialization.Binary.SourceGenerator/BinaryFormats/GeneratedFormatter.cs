using System.Collections.Generic;
using Decuplr.Serialization.Analyzer.BinaryFormat;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal struct GeneratedParser {
        public TypeFormatLayout TypeInfo { get; set; }
        public IReadOnlyList<GeneratedSourceCode> GeneratedSourceCodes { get; set; }
        public string ParserClassName { get; set; }
        public string ParserSourceText { get; set; }
    }

}
