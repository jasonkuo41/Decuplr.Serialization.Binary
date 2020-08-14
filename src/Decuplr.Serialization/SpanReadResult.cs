using System;

namespace Decuplr.Serialization {
    /// <summary>
    /// Represents a generic deserialization result for <see cref="ReadOnlySpan{T}"/>
    /// </summary>
    /// <typeparam name="T">The deserialized type</typeparam>
    public readonly struct SpanReadResult<T> {
        /// <summary>
        /// The total bytes that was consumed, -1 if there is insufficient data
        /// </summary>
        public int Consumed { get; }

        /// <summary>
        /// If there should be more data in place before the deserialization can success
        /// </summary>
        public bool HasInsufficientData => Consumed < 0;

        /// <summary>
        /// The result of the deserialization
        /// </summary>
        public T Result { get; }

        /// <summary>
        /// Creates the result of the deserialization
        /// </summary>
        /// <param name="result">The result object</param>
        /// <param name="consumed">The bytes consumed, -1 if there is insufficient data</param>
        public SpanReadResult(T result, int consumed) {
            Result = result;
            Consumed = consumed;
        }
    }
}
