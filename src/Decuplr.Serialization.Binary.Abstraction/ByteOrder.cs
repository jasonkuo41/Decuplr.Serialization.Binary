
namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Specifies the byte order (endianess) of a member
    /// </summary>
    public enum ByteOrder {
        /// <summary>
        /// The least significant byte (lowest address) is stored first.
        /// </summary>
        LittleEndian,

        /// <summary>
        /// The most significant byte (highest address) is stored first.
        /// </summary>
        BigEndian
    }
}
