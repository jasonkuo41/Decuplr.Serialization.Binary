using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class BinaryFormatAttribute : Attribute {
        public BinaryFormatAttribute() {

        }
    }
}
