using System;

namespace Decuplr.Serialization.Binary {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class BinaryFormatAttribute : Attribute {

        /// <summary>
        /// Declare this class or struct to be able to serialize to binary format, with layout as <see cref="BinaryLayout.Auto"/>
        /// </summary>
        public BinaryFormatAttribute() : this (BinaryLayout.Auto) { }

        /// <summary>
        /// Declare this class or struct to be able to serialize to binary format
        /// </summary>
        /// <param name="layout">The target layout</param>
        public BinaryFormatAttribute(BinaryLayout layout) {
            Layout = layout;
        }

        public BinaryLayout Layout { get; }
    }
}
