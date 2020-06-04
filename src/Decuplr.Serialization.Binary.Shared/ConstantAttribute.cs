using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple =false, Inherited = false)]
    public sealed class ConstantAttribute : Attribute {
        public ConstantAttribute() { }

        public bool NeverVerify { get; set; } = false;
    }
}
