using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator.Schemas {
    internal class ParserProviderWrapper {

        private readonly ITypeSymbol ParsedType;
        private readonly string ParserName;

        public ParserProviderWrapper(ITypeSymbol parsedType, string parserName) {
            ParsedType = parsedType;
            ParserName = parserName;
        }

        public EmbeddedCode Provide(EmbeddedCode parserClass, out IParserKindProvider kindProvider) {
            var parserProviderName = $"{ParserName}_Provider";

            var node = new CodeNodeBuilder();

            // Add the parser to our source, not nested but outside
            node.AddPlain(parserClass.SourceCode);

            node.AddNode($"private class {parserProviderName} : IParserProvider<{ParsedType}>", node => {
                node.AddNode($"public TypeParser<{ParsedType}> ProvideParser(IParserDiscovery discovery)", node => {
                    node.AddStatement($"return new {ParserName}(discovery)");
                });

                node.AddNode($"public bool TryProvideParser(IParserDiscovery discovery, out TypeParser<{ParsedType}> parser)", node => {
                    node.AddStatement($"parser = new {ParserName}(discovery, out var isSuccess)");
                    node.AddNode("if(!isSuccess)", node => {
                        // we don't want a half baked parser to be returned and possibly used
                        node.AddStatement("parser = null");
                    });
                    node.AddStatement($"return isSuccess");
                });
            });
            kindProvider = new ParserProviderKindProvider(ParsedType, parserProviderName);
            return new EmbeddedCode {
                CodeNamespaces = parserClass.CodeNamespaces,
                SourceCode = node.ToString(),
            };
        }
    }
}
