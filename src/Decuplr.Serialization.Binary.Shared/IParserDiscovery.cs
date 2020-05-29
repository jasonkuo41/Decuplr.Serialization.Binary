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
        /// <param name="shouldPrioritize">Should the namespace override other namespace that has been established</param>
        /// <returns>A cloned parser discovery that discovers the new included namespace</returns>
        IParserDiscovery WithNamespace(IEnumerable<string> parserNamespace, bool shouldPrioritize = false);

        /// <summary>
        /// Makes a copy of the discovery, and adds the given namespace for discovering types
        /// </summary>
        /// <param name="parserNamespace">The given namespace</param>
        /// <param name="shouldPrioritize">Should the namespace override other namespace that has been established</param>
        /// <returns>A cloned parser discovery that discovers the new included namespace</returns>
        IParserDiscovery WithNamespace(string parserNamespace, bool shouldPrioritize = false);
    }
}
