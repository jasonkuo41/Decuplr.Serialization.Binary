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
        /// <param name="currentNamespace">The current namespace</param>
        /// <param name="rootNamespace">The root namespace</param>
        /// <returns>The target parser</returns>
        TypeParser<T> ProvideParser(IParserNamespace currentNamespace, INamespaceRoot rootNamespace);

        /// <summary>
        /// Try to create a parser for the current namespace
        /// </summary>
        /// <param name="currentNamespace">The current namespace</param>
        /// <param name="rootNamespace">The root namespace</param>
        /// <param name="parser">The target parser</param>
        /// <returns>If parser is succefully delivered</returns>
        bool TryProvideParser(IParserNamespace currentNamespace, INamespaceRoot rootNamespace, out TypeParser<T> parser);

    }

}
