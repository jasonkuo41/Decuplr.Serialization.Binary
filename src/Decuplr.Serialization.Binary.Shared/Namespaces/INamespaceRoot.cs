using System;
using System.Collections;
using System.Collections.Generic;

namespace Decuplr.Serialization.Binary.Namespaces {
    /// <summary>
    /// Represents a neutral, unmodified and root of parsers namespace
    /// </summary>
    public interface INamespaceRoot {

        /// <summary>
        /// The default namespace
        /// </summary>
        IDefaultParserNamespace DefaultNamespace { get; }

        /// <summary>
        /// Create a namespace that adds to this root
        /// </summary>
        /// <param name="parserNamespace">The name of the namespace</param>
        /// <returns>A ownership of the namespace</returns>
        IParserNamespaceOwner CreateNamespace(string parserNamespace);

        /// <summary>
        /// Creates a parser discovery from the default namespace
        /// </summary>
        /// <returns>A new parser collection</returns>
        IParserDiscovery CreateDiscovery();

        /// <summary>
        /// Creates a parser discovery from the given namespaces, search priority according to it's order
        /// </summary>
        /// <param name="namespaceSources">The namespaces that are used to search for a specific parser</param>
        /// <returns>A new parser collection</returns>
        IParserDiscovery CreateDiscovery(IEnumerable<string> namespaceSources);
    }

}
