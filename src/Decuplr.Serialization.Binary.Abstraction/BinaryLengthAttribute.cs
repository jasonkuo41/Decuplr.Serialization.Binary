using System;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Uses the member as a reference for how many bytes a certain member should occupy, may fill remainders with zeros if <see cref="Alignment"/> is not set to <see cref="MemoryAlign.None"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class BinaryLengthAttribute : Attribute {
        
        /// <summary>
        /// Uses a member as a reference for the binary length
        /// </summary>
        /// <param name="lengthProvider">The referencing member</param>
        public BinaryLengthAttribute(string lengthProvider) {
            LengthProvider = lengthProvider;
        }

        /// <summary>
        /// Uses a constant number as a reference for the binary length
        /// </summary>
        /// <param name="fixedLength">The constant length</param>
        public BinaryLengthAttribute(int fixedLength) {
            FixedLength = fixedLength;
        }

        /// <summary>
        /// The defined constant binary length, null if not defined
        /// </summary>
        public int? FixedLength { get; }

        /// <summary>
        /// The reference member providing the binary length, null if not defined
        /// </summary>
        public string LengthProvider { get; }

        /// <summary>
        /// The memory aligment mode when there are remain empty spaces, <see cref="MemoryAlign.None"/> if it should always fill.
        /// </summary>
        public MemoryAlign Alignment { get; set; }
    }
}
