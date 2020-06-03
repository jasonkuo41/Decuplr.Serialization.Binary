using Decuplr.Serialization.Analyzer.BinaryFormat;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal struct GeneratedParser {

        public GeneratedParser(TypeFormatLayout type, string className, string sourceText) {
            TypeInfo = type;
            ParserClassName = className;
            ParserSourceText = sourceText;
        }

        public TypeFormatLayout TypeInfo { get; set; }
        public string ParserClassName { get; set; }
        public string ParserSourceText { get; set; }
    }

}
