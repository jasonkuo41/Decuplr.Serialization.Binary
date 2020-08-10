using System;

namespace Decuplr.Serialization {

    /// <summary>
    /// Format the current type using another type, it may be an intermediate tpye or a base type of the current type
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class FormatAsAttribute : Attribute {

        /// <summary>
        /// Format the current type using another type.
        /// </summary>
        /// <param name="type">The target type for actual serialization</param>
        public FormatAsAttribute(Type type) {
            FormatType = type;
        }

        /// <summary>
        /// Format the current type using another type when <paramref name="valueSource"/> can be evaluated to true
        /// </summary>
        /// <param name="type">The target type for actual serialization</param>
        /// <param name="valueSource"></param>
        public FormatAsAttribute(Type type, string valueSource) {
            FormatType = type;
            Condition = new ConditionExpression(valueSource);
        }

        public FormatAsAttribute(Type type, string valueSource, object value) {
            FormatType = type;
            Condition = new ConditionExpression(valueSource) {
                ComparedValue = value
            };
        }

        public FormatAsAttribute(Type type, string valueSource, Condition condition, object value) {
            FormatType = type;
            Condition = new ConditionExpression(valueSource) {
                Condition = condition,
                ComparedValue = value
            };
        }

        /// <summary>
        /// The type that will be formatted into
        /// </summary>
        public Type FormatType { get; }

        /// <summary>
        /// The condition that only formats as other when evaluated to true
        /// </summary>
        public ConditionExpression? Condition { get; }
    }
}
