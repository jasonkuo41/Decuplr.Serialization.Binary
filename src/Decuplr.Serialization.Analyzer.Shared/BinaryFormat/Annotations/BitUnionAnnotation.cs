namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    public class BitUnionAnnotation : TypeDecisionAnnotation {

        public int UnionSize { get; }

        public int[][] UnionLayout { get; }
    }

}
