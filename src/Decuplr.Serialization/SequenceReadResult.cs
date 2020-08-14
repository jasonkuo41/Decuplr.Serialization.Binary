using System;
using System.Buffers;

namespace Decuplr.Serialization {
    /// <summary>
    /// Represents a generic deserialization result for <see cref="ReadOnlySequence{T}"/>
    /// </summary>
    /// <typeparam name="T">The deserialized type</typeparam>
    public readonly struct SequenceReadResult<T> {
        /// <summary>
        /// The total bytes that was consumed, -1 if there is insufficient data
        /// </summary>
        public long Consumed { get; }

        /// <summary>
        /// Gets the current <see cref="SequencePosition"/> of consumed data
        /// </summary>
        public SequencePosition Position { get; }

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
        public SequenceReadResult(T result, long consumed, SequencePosition position) {
            Result = result;
            Consumed = consumed;
            Position = position;
        }
    }
}
