using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decuplr.Serialization.Namespaces {
    /// <summary>
    /// Represents a standalone containers that allow different features for the same type to coexist using the namespace concept.
    /// </summary>
    public interface INamespaceNode : IReadOnlyNamespaceNode {

        /// <inheritdoc cref="IReadOnlyNamespaceNode.Parent"/>
        new INamespaceNode? Parent { get; }

        /// <inheritdoc cref="IReadOnlyNamespaceNode.ChildNodes"/>
        new IReadOnlyDictionary<string, INamespaceNode> ChildNodes { get; }

        /// <summary>
        /// Retrieves the requested item from the collection.
        /// </summary>
        /// <typeparam name="TItem">The item key.</typeparam>
        /// <param name="assembly">The calling assembly.</param>
        /// <returns>The requested item, or null if it is not present.</returns>
        new object this[Assembly assembly, Type type] { get; set; }

        /// <summary>
        /// Sets the given item along with the assembly to the namespace.
        /// </summary>
        /// <typeparam name="TItem">The item key.</typeparam>
        /// <param name="instance">The item value.</param>
        void Set<TKind>(Assembly assembly, TKind item);

        /// <summary>
        /// Get's the children namespace, creates a new <see cref="INamespaceNode"/> if doesn't exist.
        /// </summary>
        /// <param name="namespaceName">The child namespace.</param>
        /// <returns>The corresponding namespace node.</returns>
        INamespaceNode GetChildNamespace(string namespaceName);
    }
}
