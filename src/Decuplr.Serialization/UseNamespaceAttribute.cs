using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization {

    /// <summary>
    /// Applies namespace to the whole type or a specific member
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class UseNamespaceAttribute : Attribute {
        /// <summary>
        /// Applies a namespace to the whole type or a specific member
        /// </summary>
        /// <param name="namespaceName">The namespace</param>
        public UseNamespaceAttribute(string namespaceName) {
            Namespace = namespaceName;
        }

        /// <summary>
        /// The applied namespace
        /// </summary>
        public string Namespace { get; }
    }
}
