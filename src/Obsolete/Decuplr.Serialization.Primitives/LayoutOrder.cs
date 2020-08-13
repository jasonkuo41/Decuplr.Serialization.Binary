
namespace Decuplr.Serialization {
    /// <summary>
    /// Determines the type's layout out for the designated type
    /// </summary>
    public enum LayoutOrder : byte {
        /// <summary>
        /// If any ordering attribute is present in the class or struct, it automatically becomes <see cref="Explicit"/>, otherwise <see cref="Sequential"/>
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
