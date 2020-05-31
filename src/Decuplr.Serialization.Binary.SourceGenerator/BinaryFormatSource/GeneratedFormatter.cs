namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal struct GeneratedParser {
        public GeneratedParserType ParserType { get; set; }
        public string? ParserClassName { get; set; }
        public string? ParserSourceText { get; set; }
    }

    internal enum GeneratedParserType {
        SealedParser,
        SolidParserProvider,
        GenericParserProvider
    }
}
