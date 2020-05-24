using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class NetworkByteOrderAttribute : Attribute {
        public NetworkByteOrderAttribute() {}
    }

}
