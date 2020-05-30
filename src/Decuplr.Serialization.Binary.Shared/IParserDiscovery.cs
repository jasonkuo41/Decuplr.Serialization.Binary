using System.Collections.Generic;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Describes a service that allows discovery of parsers within the given namespace
    /// </summary>
    public interface IParserDiscovery : IParserCollection {
        /// <summary>
        /// Makes a copy of the discovery, and adds the given namespace for discovering types
        /// </summary>
        /// <param name="parserNamespace">The given namespace</param>
        /// <returns>A cloned parser discovery that discovers the new included namespace</returns>
        IParserDiscovery WithNamespace(IEnumerable<string> parserNamespace);

        /// <summary>
        /// Makes a copy of the discovery, and adds the given namespace for discovering types
        /// </summary>
        /// <param name="parserNamespace">The given namespace</param>
        /// <returns>A cloned parser discovery that discovers the new included namespace</returns>
        IParserDiscovery WithNamespace(string parserNamespace);

        /// <summary>
        /// Makes a copy of the discovery, and add the given namespace to priotize over other namepace for discoverying types
        /// </summary>
        /// <param name="parserNamespace">The given namespace</param>
        /// <returns>A cloned parser discovery that discovers the new included namespace</returns>
        IParserDiscovery WithPrioritizedNamespace(IEnumerable<string> parserNamespace);

        /// <summary>
        /// Makes a copy of the discovery, and add the given namespace to priotize over other namepace for discoverying types
        /// </summary>
        /// <param name="parserNamespace">The prioritized namespace</param>
        /// <returns>A cloned parser discovery that discovers the new included namespace</returns>
        IParserDiscovery WithPrioritizedNamespace(string parserNamespace);

    }
}
