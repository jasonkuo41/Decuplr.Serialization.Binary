using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization {
    /// <summary>
    /// Annotates a member with the order of a layout, depending on the context it may represent relative or absolute index
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class IndexAttribute : Attribute {

        /// <summary>
        /// Annotates a member with the order a layout
        /// </summary>
        /// <param name="index">The order of the layout</param>
        public IndexAttribute(int index) {
            Index = index;
        }

        /// <summary>
        /// The order of the layout
        /// </summary>
        public int Index { get; }
    }
}
