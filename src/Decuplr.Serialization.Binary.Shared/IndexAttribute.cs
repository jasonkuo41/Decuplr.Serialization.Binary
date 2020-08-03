using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class IndexAttribute : Attribute {
        public IndexAttribute(int index) {
            Index = index;
        }

        public int Index { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class UnionAttribute : Attribute {
        public UnionAttribute(params int[] occupybits) {

        }

        /// <summary>
        /// Describe if the union result in not a octet and needs to be filled, where should the data allign to.
        /// </summary>
        public BitAllignment Allignment { get; set; }

        /// <summary>
        /// The fixed size for this union, missing bits will be filled.
        /// </summary>
        public int FixedSize { get; set; }

        /// <summary>
        /// Checks if the union results in n * octets (n byte). If set false, missing bits will be filled with zero. 
        /// When fixed size is set, this check is garuantee to pass.
        /// </summary>
        public bool AllignmentCheck { get; set; } = false;

    }


    public enum BitAllignment {
        StartSide,
        EndSide
    }
}
