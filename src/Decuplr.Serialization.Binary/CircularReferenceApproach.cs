namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// The approach when dealing with circular references
    /// </summary>
    public enum CircularReferenceApproach {
        /// <summary>
        /// Never attempt to handle it. See <seealso cref="CircularReferenceMode.NeverDetect"/>.
        /// </summary>
        NeverDetect,

        /// <summary>
        /// Handles the reference circular problem using depth detection approach. See <seealso cref="CircularReferenceMode.HandleWithDepth(int)"/>.
        /// </summary>
        DepthOnly,

        /// <summary>
        /// Detects circular reference and throw when encountered. See <seealso cref="CircularReferenceMode.DetectAndThrow"/>
        /// </summary>
        DetectAndThrow
    }
}
