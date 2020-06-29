using Decuplr.Serialization.Binary.Annotations;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal struct Condition {

        public Condition(string sourceName) : this() {
            SourceName = sourceName;
        }

        private Operator? _operand;
        private object? _comparedValue;

        // The evaluated source
        public string SourceName { get; set; }

        // If this is null it's equal to Operand.Equal
        public Operator Operator {
            get => _operand ?? Operator.Equal;
            set => _operand = value;
        }

        // If this is null, we check if source returns bool
        public object ComparedValue {
            get => _comparedValue ?? true;
            set => _comparedValue = value;
        }
    }

}
