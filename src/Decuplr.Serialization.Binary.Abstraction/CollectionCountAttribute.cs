using System;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Uses the member as a reference for how many items a collection can contain, can only apply on collections
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CollectionCountAttribute : Attribute {

        /// <summary>
        /// Uses a member as a reference for the item count
        /// </summary>
        /// <param name="lengthProvider">The referencing member</param>
        public CollectionCountAttribute(string lengthProvider) {
            LengthProvider = lengthProvider;
        }

        /// <summary>
        /// Uses a constant number as a reference for the item count
        /// </summary>
        /// <param name="fixedCount">The constant count</param>
        public CollectionCountAttribute(int fixedCount) {
            FixedCount = fixedCount;
        }

        /// <summary>
        /// The defined constant item count, null if not defined
        /// </summary>
        public int? FixedCount { get; }

        /// <summary>
        /// The reference member providing the item count, null if not defined
        /// </summary>
        public string LengthProvider { get; }

    }
}
