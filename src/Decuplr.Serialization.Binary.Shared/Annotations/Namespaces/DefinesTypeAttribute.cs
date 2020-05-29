using System;

namespace Decuplr.Serialization.Binary.Annotations.Namespaces {
    /// <summary>
    /// This attribute implies to prioritize the type with the namespace, must be combined with <see cref="ApplyNamespaceAttribute"/> to be meaningful
    /// </summary>

    // TODO : [QI] Generate a compiler warning if this is not attached along with ApplyNamespace

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class DefinesTypeAttribute : Attribute { 
        public DefinesTypeAttribute(params Type[] type) {
            DefinedTypes = type;
        }

        public Type[] DefinedTypes { get; }
    }
}
