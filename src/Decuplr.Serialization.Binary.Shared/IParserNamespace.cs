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

    /// <summary>
    /// Describes a collection of parsers
    /// </summary>
    public interface IParserCollection {

        /// <summary>
        /// Get's the legnth provider for the collection
        /// </summary>
        LengthProvider? LengthProvider { get; }

        /// <summary>
        /// Attempts to get a parser from the namespace
        /// </summary>
        /// <typeparam name="T">The type being parsed</typeparam>
        /// <param name="parser">The parser for this type</param>
        /// <returns>If the parser is correctly located or created</returns>
        bool TryGetParser<T>(out TypeParser<T> parser);

        /// <summary>
        /// Get's a parser from the namespace
        /// </summary>
        /// <typeparam name="T">The type being parsed</typeparam>
        /// <returns>The parser for this type</returns>
        TypeParser<T> GetParser<T>();

    }
}
