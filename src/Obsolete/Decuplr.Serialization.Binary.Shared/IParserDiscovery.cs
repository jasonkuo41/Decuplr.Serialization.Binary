using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Describes a service that allows discovery of parsers within the given namespace
    /// </summary>
    public interface IParserDiscovery : IParserCollection {

        /// <summary>
        /// The current binary layout (endianess) of the discovery
        /// </summary>
        ByteOrder ByteOrder { get; }

        /// <summary>
        /// The current text encoding of the discovery
        /// </summary>
        Encoding TextEncoding { get; }

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
