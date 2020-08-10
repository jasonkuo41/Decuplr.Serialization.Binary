namespace Decuplr.Serialization {

    /// <summary>
    /// An expression the describes condition
    /// </summary>
    public struct ConditionExpression {

        /// <summary>
        /// Creates a expression using the default configuration
        /// </summary>
        /// <param name="sourceName">The evaluated member</param>
        public ConditionExpression(string sourceName) {
            SourceName = sourceName;
            _operand = Condition.Equal;
            _comparedValue = true;
        }

        private Condition? _operand;
        private object? _comparedValue;

        /// <summary>
        /// The evaluated member
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// The condition operator
        /// </summary>
        public Condition Condition {
            get => _operand ?? Condition.Equal;
            set => _operand = value;
        }

        /// <summary>
        /// The value being compared
        /// </summary>
        public object ComparedValue {
            get => _comparedValue ?? true;
            set => _comparedValue = value;
        }
    }

}
