using System;

namespace Decuplr.Serialization.Binary {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class BinaryFormatAttribute : Attribute {

        /// <summary>
        /// Declare this class or struct to be able to serialize to binary format, with layout as <see cref="LayoutOrder.Auto"/>
        /// </summary>
        public BinaryFormatAttribute() : this (LayoutOrder.Auto) { }

        /// <summary>
        /// Declare this class or struct to be able to serialize to binary format
        /// </summary>
        /// <param name="layout">The target layout</param>
        public BinaryFormatAttribute(LayoutOrder layout) {
            Layout = layout;
        }

        public LayoutOrder Layout { get; }

        /// <summary>
        /// Declaring the format is final, and cannot be modified by all means.
        /// </summary>
        /// <remarks>
        /// For example if a field has network byte order applied, then outer class cannot override that behaviour, even with [Override] attribute
        /// </remarks>
        public bool Sealed { get; set; }

        /// <summary>
        /// Never deserialize the type, doing so will immediately throw <see cref="InvalidOperationException"/>
        /// </summary>
        /// <remarks>
        /// This is useful for types that have readonly evaluation and data cannot be reversed back
        /// </remarks>
        public bool NeverDeserialize { get; set; }

    }
}
