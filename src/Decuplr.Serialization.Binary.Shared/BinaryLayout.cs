namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Determines the type layout for the binary packer to read
    /// </summary>
    public enum BinaryLayout : byte {
        /// <summary>
        /// If any index is present in the class or struct, it automatically becomes <see cref="Explicit"/>, otherwise <see cref="Sequential"/>
        /// </summary>
        Auto,

        /// <summary>
        /// The layout of the class or struct is sequentially formatted
        /// </summary>
        Sequential,

        /// <summary>
        /// The layout of the class or struct should be explicitly defined
        /// </summary>
        Explicit
    }
}
