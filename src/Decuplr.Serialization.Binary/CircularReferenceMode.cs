using System;
using System.Collections.Generic;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Defines the behaviour of circular references.
    /// </summary>
    public readonly struct CircularReferenceMode {

        private CircularReferenceMode(CircularReferenceApproach approach, int maxDepthCount) {
            Approach = approach;
            MaxDepthCount = maxDepthCount;
        }

        /// <summary>
        /// Defines the approach when it comes to circular reference.
        /// </summary>
        public CircularReferenceApproach Approach { get; }

        /// <summary>
        /// Defines the maximum depth allowed when serializing or deserializing the binary data.
        /// when <see cref="Approach"/> is set to <see cref="CircularReferenceApproach.DepthOnly"/>
        /// </summary>
        public int MaxDepthCount { get; }

        /// <summary>
        /// Never attempt to detect any form of circular reference. 
        /// If a circular reference is present during serialization, <see cref="StackOverflowException"/> may be thrown.
        /// This option exchanges speed for safety.
        /// </summary>
        public static CircularReferenceMode NeverDetect { get; } = default;

        /// <summary>
        /// Handle the circular reference with depth detection.
        /// This option is a balance between safety and performance.
        /// </summary>
        /// <remarks>
        /// May cause custom implemetation of <see cref="LinkedList{T}"/> to throw exception when <see cref="IEnumerable{T}.GetEnumerator"/> is not present
        /// and when <paramref name="depthCount"/> is set to a low value.
        /// </remarks>
        /// <param name="depthCount">The depth count, when set to value lesser or equal to zero, a default value will be used</param>
        /// <returns>The reference mode instance</returns>
        public static CircularReferenceMode HandleWithDepth(int depthCount) {
            depthCount = depthCount <= 0 ? 64 : depthCount;
            return new CircularReferenceMode(CircularReferenceApproach.DepthOnly, depthCount);
        }

        /// <summary>
        /// Detects circular reference using <see cref="object.Equals(object)"/> with only reference types and throws <see cref="SerializationFaultException"/> when a circular reference is detected.
        /// This option is the slowest but ensure correctness of the serialization.
        /// </summary>
        public static CircularReferenceMode DetectAndThrow { get; } = new CircularReferenceMode(CircularReferenceApproach.DetectAndThrow, 0);
    }
}
