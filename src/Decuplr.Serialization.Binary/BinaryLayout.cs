namespace Decuplr.Serialization.Binary {
    public enum BinaryLayout : byte {
        /// <summary>
        /// If index is present in the class or struct, it automatically becomes <see cref="Explicit"/>, otherwise <see cref="Sequential"/>
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
