using System;

namespace Decuplr.Serialization.Binary.Annotations.Namespaces {
    /// <summary>
    /// This attribute implies that the namespaces defines all primitive types and should prioritize this namespace
    /// </summary>

    // TODO : [QI] Generate a compiler warning if this is not attached along with ApplyNamespace

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DefinesPrimitivesAttribute : Attribute {
        public DefinesPrimitivesAttribute() { }
    }
}
