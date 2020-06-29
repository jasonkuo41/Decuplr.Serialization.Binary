using Decuplr.Serialization.Binary.Analyzers;
using Decuplr.Serialization.Binary.Annotations;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    public struct Condition {
        // The evaluated source
        public AnalyzedMember Source { get; set; }

        // If this is null it's equal to Operand.Equal
        public Operator? Operand { get; set; }

        // If this is null, we check if source returns bool
        public object? ComparedValue { get; set; }
    }

}
