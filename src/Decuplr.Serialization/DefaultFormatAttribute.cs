using System;
using Decuplr.Serialization.Namespaces;

namespace Decuplr.Serialization {
    /// <summary>
    /// Marks the underlying data should use default formatting and will not be overriden 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    //[ApplyNamespace("Default")]
    public sealed class DefaultFormatAttribute : Attribute { }

}
