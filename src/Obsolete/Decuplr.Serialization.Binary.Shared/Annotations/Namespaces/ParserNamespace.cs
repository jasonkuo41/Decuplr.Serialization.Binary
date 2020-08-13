using System;

namespace Decuplr.Serialization.Binary.Annotations.Namespaces {

    /// <summary>
    /// Marks the parser to load into certain namespace
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class BinaryParserNamespaceAttribute : Attribute {
        public BinaryParserNamespaceAttribute(string targetNamespace) {
            Namespace = targetNamespace;
        }

        public string Namespace { get; }
    }

}
