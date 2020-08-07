using System;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Allows a value tuple to be a serialized as bit fields, it's final product would always be octet-aligned
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class BitUnionAttribute : Attribute {

        /// <summary>
        /// Set's how many bits each value tuple element should occpuy
        /// </summary>
        /// <param name="occupybits">The array indicating the corresponding bit amount a tuple element should occupy</param>
        public BitUnionAttribute(params int[] occupybits) {
            OccupiedBits = occupybits;
        }

        /// <summary>
        /// When the bit-union is not octet-aligned and needs to be filled, the aligment rule it should follow. When set to <see cref="BitAlign.None"/>, the occupying bits must be a octet-aligned.
        /// </summary>
        public BitAlign Alignment { get; set; }

        /// <summary>
        /// When the bit-union is not octet-aligned, the value it should fill in with. Either true flag (1) or false flag (0).
        /// </summary>
        public bool FillOnes { get; set; }

        /// <summary>
        /// The occuping bits that this bit union represents
        /// </summary>
        public int[] OccupiedBits { get; }
    }
}
