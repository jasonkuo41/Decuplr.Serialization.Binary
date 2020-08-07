namespace Decuplr.Serialization.Binary {

    /// <summary>
    /// Describes where the memory should align to
    /// </summary>
    public enum BitAlign {
        /// <summary>
        /// The data should not or is not obligated for alignment
        /// </summary>
        None,

        /// <summary>
        /// Align the memory to the lower address side (starting point)
        /// </summary>
        LowAddress,

        /// <summary>
        /// Align the memory to the higher address side (ending point)
        /// </summary>
        HighAddress
    }
}
