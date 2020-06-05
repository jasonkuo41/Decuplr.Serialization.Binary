using System;

namespace Decuplr.Serialization.Binary.Annotations.Namespaces {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ApplyNamespaceIfAttribute : Attribute {
        public ApplyNamespaceIfAttribute(string formatterNamespace, object firstConsturctor, params object[] matchConstructors) {
            FormatterNamespace = formatterNamespace;
        }

        public string FormatterNamespace { get; }
    }
}
