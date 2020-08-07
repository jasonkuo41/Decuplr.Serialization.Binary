using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Annotate this member to be ignored when order or the member to be serialized are not explicitly stated
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreAttribute : Attribute { }
}
