namespace Decuplr.Serialization {
    /// <summary>
    /// Describes the conclusion of the deserialization result
    /// </summary>
    public enum DeserializeConclusion : byte {
        /// <summary>
        /// There was insufficient data to deserialize, receive more and retry
        /// </summary>
        InsufficientSize,

        /// <summary>
        /// The data included invalid data or state and shall not be deserialized
        /// </summary>
        Faulted,

        /// <summary>
        /// The data deserialized successfully
        /// </summary>
        Success,
    }
}
