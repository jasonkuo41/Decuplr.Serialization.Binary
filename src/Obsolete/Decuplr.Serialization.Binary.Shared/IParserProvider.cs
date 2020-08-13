using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Provides a parser for the current namespace
    /// </summary>
    /// <typeparam name="T">The type that can be parsed</typeparam>
    public interface IParserProvider<T> {

        /// <summary>
        /// Creates the parser for the current namespace
        /// </summary>
        /// <param name="discovery">The current namespace</param>
        /// <returns>The target parser</returns>
        TypeParser<T> ProvideParser(IParserDiscovery discovery);

        /// <summary>
        /// Try to create a parser for the current namespace
        /// </summary>
        /// <param name="discovery">The current namespace</param>
        /// <param name="parser">The target parser</param>
        /// <returns>If parser is successfully delivered</returns>
        bool TryProvideParser(IParserDiscovery discovery, out TypeParser<T> parser);

    }

}
