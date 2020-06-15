using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.Schemas {
    /// <summary>
    /// Only provides parser wrapper for those who has constructor (IParserDiscovery) and (IParserDiscovery, out bool)
    /// </summary>
    internal class GenericParserProviderWrapper {

        private readonly INamedTypeSymbol ParsedType;
        private readonly string ParserName;

        public GenericParserProviderWrapper(INamedTypeSymbol parsedType, string parserName) {
            ParsedType = parsedType;
            ParserName = parserName;
        }

        public EmbeddedCode Provide(EmbeddedCode parserClass, out IParserKindProvider provider) {
            var parserProviderName = $"{ParserName}_GenericProvider";
            var genParam = ParsedType.TypeParameters;
            if (genParam.Length == 0 || !ParsedType.IsUnboundGenericType)
                throw new ArgumentException("Symbol is not a generic type");

            var node = new CodeNodeBuilder();
            // TODO : Please make sure this works, I have no doc over this!! (or at least it's undoced)
            // a roslyn guy says it'll work, so I trust him
            node.AddNode($"private class {parserProviderName}<{string.Join(",", genParam.Select(x => x.ToString()))}> : GenericParserProvider", node => {
                // Embed the source code in this class
                node.AddPlain(parserClass.SourceCode);

                node.AddNode("public override TypeParser ProvideParser(IParserDiscovery discovery)", node => {
                    node.AddStatement($"return new {ParserName}(discovery)");
                });

                node.AddNode("public override bool TryProvideParser(IParserDiscovery discovery, out TypeParser parser)", node => {
                    node.AddStatement($"parser = new {ParserName}(discovery, out var isSuccess)");
                    node.AddNode("if(!isSuccess)", node => {
                        // we don't want a half baked parser to be returned and possibly used
                        node.AddStatement("parser = null");
                    });
                    node.AddStatement($"return isSuccess");
                });
            });

            provider = new GenericParserKindProvider(ParsedType, parserProviderName);
            return new EmbeddedCode {
                CodeNamespaces = parserClass.CodeNamespaces,
                SourceCode = node.ToString()
            };
        }
    }
}
