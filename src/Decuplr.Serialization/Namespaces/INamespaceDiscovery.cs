using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Namespaces {

    /// <summary>
    /// A discovery service that is able to traverse through various namespaces with priority and order in mind to fetch the requested item
    /// </summary>
    public interface INamespaceDiscovery {
        /// <summary>
        /// The namespaces the discovery service would traverse through
        /// </summary>
        IEnumerable<INamespaceContainer> TraversingNamespaces { get; }

        /// <summary>
        /// Makes a copy of the discovery, and adds the given namespace for traversing
        /// </summary>
        /// <param name="namespaceNames">The given namespaces</param>
        /// <returns>A cloned discovery that will traverse the new included namespace</returns>
        INamespaceDiscovery WithNamespace(IEnumerable<string> namespaceNames);

        /// <summary>
        /// Makes a copy of the discovery, and adds the given namespace for traversing
        /// </summary>
        /// <param name="namespaceName">The given namespace</param>
        /// <returns>A cloned discovery that will traverse the new included namespace</returns>
        INamespaceDiscovery WithNamespace(string namespaceName);

        /// <summary>
        /// Makes a copy of the discovery, and add the given namespace to priotize over other namepace for traversing
        /// </summary>
        /// <param name="namespaceNames">The prioritized namespaces</param>
        /// <returns>A cloned discovery that will traverse the new included namespace</returns>
        INamespaceDiscovery WithPrioritizedNamespace(IEnumerable<string> namespaceNames);

        /// <summary>
        /// Makes a copy of the discovery, and add the given namespace to priotize over other namepace for travesing
        /// </summary>
        /// <param name="namespaceName">The prioritized namespace</param>
        /// <returns>A cloned discovery that will traverse the new included namespace</returns>
        INamespaceDiscovery WithPrioritizedNamespace(string namespaceName);
    }
}
