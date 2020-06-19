namespace Decuplr.Serialization.Binary.Templates.ParserSource {
    internal interface IParserDependencyComponent {
        string TypeName { get; }

        string GetInitializer(ParserDiscoveryArgs args);

        string TryGetInitializer(ParserDiscoveryArgs args, string parserName);
    }
}
