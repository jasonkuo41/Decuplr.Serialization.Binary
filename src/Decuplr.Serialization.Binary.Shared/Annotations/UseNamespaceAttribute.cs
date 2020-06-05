using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.Annotations {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class UseNamespaceAttribute : Attribute {
    }
}
