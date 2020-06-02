namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    public class FormatAsAnnotation : TypeDecisionAnnotation {
        /// <summary>
        /// 
        /// </summary>
        public Condition? Codition { get; }

        /// <summary>
        /// The type to be formatted as
        /// </summary>
        public AnalyzedType FormatAs { get; }
    }

}
