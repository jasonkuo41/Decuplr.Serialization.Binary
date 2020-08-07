using System;

namespace Decuplr.Serialization {
    /// <summary>
    /// Describes the condition evaluation kind
    /// </summary>
    public enum Condition {
        /// <summary>
        /// Evaluates if the source equals to the target using <see cref="IEquatable{T}.Equals(T)"/> whenever possible, otherwise <see cref="object.Equals(object)"/> will be used
        /// </summary>
        Equal,

        /// <summary>
        /// Evaluates if the source doesn't equal to the target using <see cref="IEquatable{T}.Equals(T)"/> whenever possible, otherwise <see cref="object.Equals(object)"/> will be used for evaluation
        /// </summary>
        NotEqual,

        /// <summary>
        /// Evaluates if the source is greater than the target using <see cref="IComparable{T}.CompareTo(T)"/> whenever possible, otherwise <see cref="IComparable.CompareTo(object)"/> will be used. True when the returning value is > 1.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Evaluates if the source is greater than or equal to the target using <see cref="IComparable{T}.CompareTo(T)"/> whenever possible, otherwise <see cref="IComparable.CompareTo(object)"/> will be used. True when the returning value is >= 1.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Evaluates if the source is less than to the target using <see cref="IComparable{T}.CompareTo(T)"/> whenever possible, otherwise <see cref="IComparable.CompareTo(object)"/> will be used. True when the returning value is < 1.
        /// </summary>
        LessThan,

        /// <summary>
        /// Evaluates if the source is less than or equal to the target using <see cref="IComparable{T}.CompareTo(T)"/> whenever possible, otherwise <see cref="IComparable.CompareTo(object)"/> will be used. True when the returning value is <= 1.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Evaluates if the source type is a derived type of the provided type
        /// </summary>
        IsTypeOf,

        /// <summary>
        /// Evaluates if the source type is not a derived type of the provided type
        /// </summary>
        IsNotTypeOf,
    }
}
