namespace Decuplr.Serialization.Binary {
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
