using System;
using Decuplr.Serialization.Binary.Annotations.Namespaces;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Marks the underlying data should use default formatting and will not be overriden 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [ApplyNamespace("Default")]
    [DefinesPrimitives]
    public sealed class DefaultFormatAttribute : Attribute {
        public DefaultFormatAttribute() { }
    }
}
