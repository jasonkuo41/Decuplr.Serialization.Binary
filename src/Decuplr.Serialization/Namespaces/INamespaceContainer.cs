using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decuplr.Serialization.Namespaces {

    /// <summary>
    /// Represents a standalone containers that allow different features for the same type to coexist using the namespace concept
    /// </summary>
    public interface INamespaceContainer : IEnumerable<KeyValuePair<TypeEntryInfo, object>> {

        /// <summary>
        /// Incremented for each modification and can be used to verify cached results.
        /// </summary>
        int Revision { get; }

        /// <summary>
        /// The upper heirachy of the namespace. Null if this is the highest heirachy.
        /// </summary>
        /// <remarks>For example, MyNamespace.Collections would return the namespace MyNamespace.</remarks>
        INamespaceContainer? ParentNamespace { get; }

        /// <summary>
        /// The child namespaces that are one heirachy lower to this parent namespace.
        /// </summary>
        IReadOnlyDictionary<string, INamespaceContainer> ChildNamespaces { get; }

        /// <summary>
        /// The identifier of this namespace container.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// The full name (full identifier) for this namespace container
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Retrieves the requested item from the collection.
        /// </summary>
        /// <typeparam name="TItem">The item key.</typeparam>
        /// <param name="assembly">The calling assembly.</param>
        /// <returns>The requested item, or null if it is not present.</returns>
        TItem Get<TItem>(Assembly assembly) where TItem : class;

        /// <summary>
        /// Sets the given item in the collection.
        /// </summary>
        /// <typeparam name="TItem">The item key.</typeparam>
        /// <param name="instance">The item value.</param>
        void Set<TItem>(TItem instance) where TItem : class;

        /// <summary>
        /// Get's the children namespace, creates if doesn't exist
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        INamespaceContainer GetChildNamespace(string namespaceName);
    }

    public interface IReadOnlyNamespaceContainer {

    }

    public interface INamespaceTree : INamespaceNode {
        int Revision { get; }


    }

    public interface INamespaceNode {
        INamespaceTree FullTree { get; }
        INamespaceNode? Parent { get; }
        IReadOnlyDictionary<string, INamespaceNode> ChildNodes { get; }

    }

    public interface IReadOnlyNamespaceTree {

    }

    public interface IReadOnlyNamespaceNode {

    }

    public static class NamespaceExtensions {
        public static INamespaceTree Clone(this IReadOnlyNamespaceTree tree) {

        }
    }
}
