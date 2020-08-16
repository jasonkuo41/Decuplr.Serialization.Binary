using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decuplr.Serialization.Namespaces {
    /// <summary>
    /// Represents a read only standalone containers that allow different features for the same type to coexist using the namespace concept.
    /// </summary>
    public interface IReadOnlyNamespaceNode : IEnumerable<KeyValuePair<TypeEntryInfo, object>> {

        /// <summary>
        /// The root of the namespace.
        /// </summary>
        INamespaceTree Root { get; }

        /// <summary>
        /// The upper heirachy of the namespace. Null if this is the highest heirachy.
        /// </summary>
        /// <remarks>For example, MyNamespace.Collections would return the namespace MyNamespace.</remarks>
        IReadOnlyNamespaceNode? Parent { get; }

        /// <summary>
        /// The child namespaces that are one heirachy lower to this parent namespace.
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyNamespaceNode> ChildNodes { get; }

        /// <summary>
        /// The identifier of this namespace container.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// The full name (full identifier) for this namespace container.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Retrieves the requested item from the collection.
        /// </summary>
        /// <typeparam name="TItem">The item key.</typeparam>
        /// <param name="assembly">The calling assembly.</param>
        /// <returns>The requested item, or null if it is not present.</returns>
        TKind Get<TKind>(Assembly assembly);

        /// <summary>
        /// Get's the type associated with the assembly info.
        /// </summary>
        /// <param name="assembly">The assembly info part.</param>
        /// <param name="type">The type info.</param>
        /// <returns>The item that is assigned with the type and the assembly.</returns>
        object this[Assembly assembly, Type type] { get; }
    }
}
