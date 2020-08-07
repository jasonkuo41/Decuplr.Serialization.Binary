using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization {

    /// <summary>
    /// Annotates that a field is always a fixed value given the same data context. Performs equality check to determinate if the data is valid by default or if <see cref="NeverVerify"/> is false.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple =false, Inherited = false)]
    public sealed class ConstantAttribute : Attribute {
        /// <summary>
        /// Never verify the member value equals to the deserializing data
        /// </summary>
        public bool NeverVerify { get; set; } = false;
    }
}
