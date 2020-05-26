using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.Annotations.Namespaces {
    /// <summary>
    /// Marks the target attribute to apply namespace when formatting
    /// </summary>
    
    // TODO : [QI] Generate compiler warning if the attribute is not attached a attribute class
    // TODO : Allow Inherit Namespaces in the future
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ApplyNamespaceAttribute : Attribute {
        public ApplyNamespaceAttribute(string formatterNamespace) {
            FormatterNamespace = formatterNamespace;
        }

        public string FormatterNamespace { get; }
        public int PrioritizeIndex { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ApplyNamespaceIfAttribute : Attribute {
        public ApplyNamespaceIfAttribute(string formatterNamespace, object firstConsturctor, params object[] matchConstructors) {
            FormatterNamespace = formatterNamespace;
        }

        public string FormatterNamespace { get; }
        public bool DefinesPrimitives { get; set; }
    }
}
