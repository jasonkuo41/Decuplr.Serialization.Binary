using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary.Schemas {
    class TypeParserGenerator {

        private readonly TypeFormatLayout TypeInfo;
        private readonly GeneratedFormatFunction TryDeserialize;
        private readonly GeneratedFormatFunction TrySerialize;
        private readonly GeneratedFormatFunction Serialize;
        private readonly GeneratedFormatFunction GetLength;
        private readonly List<GeneratedSourceCode> AdditionalSourceCode = new List<GeneratedSourceCode>();
        
        public TypeParserGenerator(TypeFormatLayout typeInfo, IDeserializeSolution deserializeSolution, ISerializeSolution serializeSolution) {
            TypeInfo = typeInfo;

            TryDeserialize = deserializeSolution.GetDeserializeFunction();
            AdditionalSourceCode.AddRange(deserializeSolution.GetAdditionalFiles());

            TrySerialize = serializeSolution.GetTrySerializeFunction();
            Serialize = serializeSolution.GetSerializeFunction();
            GetLength = serializeSolution.GetBinaryLengthFunction();
            AdditionalSourceCode.AddRange(serializeSolution.GetAdditionalFiles());
        }

        private IParserKindProvider GetProviderFromType(string parserName, ref EmbeddedCode embeddedCode) {
            if (TypeInfo.FormatInfo.IsSealed)
                return new SealedParserKindProvider(parserName, true);
            if (TypeInfo.TypeSymbol.IsGenericType) {
                var wrapper = new GenericParserProviderWrapper(TypeInfo.TypeSymbol.ConstructUnboundGenericType(), parserName);
                embeddedCode = wrapper.Provide(embeddedCode, out var provider);
                return provider;
            }
            {
                var wrapper = new ParserProviderWrapper(TypeInfo.TypeSymbol, parserName);
                embeddedCode = wrapper.Provide(embeddedCode, out var provider);
                return provider;
            }
        }

        private void AddParserCollection(CodeNodeBuilder node) {
            node.AddNode($"internal struct {TypeInfo.GetDefaultParserCollectionName()}", node => {
                // Declare the parser we will be using
                for (var i = 0; i < TypeInfo.Member.Count; ++i) {
                    var annotation = TypeInfo.Member[i].DecisionAnnotation;
                    for (var j = 0; j < annotation.RequestParserType.Count; ++j) {
                        node.AddPlain($"public TypeParser<{annotation.RequestParserType[j]}> Parser_{i}_{j};");
                    }
                }
            });
        }

        private void AddConstructor(CodeNodeBuilder node, string? outName = null) {
            var isTryPattern = !string.IsNullOrEmpty(outName);
            if (isTryPattern)
                node.AddStatement($"{outName} = false");
            node.AddStatement("fixedSize = 0");
            for (var i = 0; i < TypeInfo.Member.Count; ++i) {
                var annotation = TypeInfo.Member[i].DecisionAnnotation;
                var currentMember = TypeInfo.Member[i];
                var usedNamespaces = TypeInfo.Member[i].UsedNamespaces;
                if (usedNamespaces.Count != 0)
                    node.AddStatement($"var parserSpace_{i} = parser.WithNamespace({string.Join(",", usedNamespaces)})");
                else
                    node.AddStatement($"var parserSpace_{i} = parser");
                for (var j = 0; j < annotation.RequestParserType.Count; ++j) {

                    if (isTryPattern) {
                        node.AddNode($"if (!parserSpace_{i}.TryGetParser(out ParserCollections.Parser_{i}_{j}))", node => {
                            node.AddStatement("return");
                        });
                    }
                    else {
                        node.AddStatement($"ParserCollections.Parser_{i}_{j} = parserSpace_{i}.GetParser<{annotation.RequestParserType[j]}>()");
                    }

                    // force capture of i
                    var captureI = i;
                    node.AddNode("if (fixedSize != null)", node => {
                        // if the member has "fixed binary length", then we can predict it's length
                        // also if we only one member in this parsers group, otherwise we can decide
                        // surely there is optimization we could do to improve this
                        // for example check for the largest fixedsize in the group
                        // but we'll ignore that for now
                        if (currentMember.ConstantLength.HasValue)
                            node.AddStatement($"fixedSize += {currentMember.ConstantLength}");
                        if (annotation.RequestParserType.Count == 1)
                            node.AddStatement($"fixedSize += ParserCollections.Parser_{captureI}_{0}.FixedSize");
                        else
                            node.AddStatement($"fixedSize = null");
                    });
                }
            }
            if (isTryPattern)
                node.AddStatement($"{outName} = true");
        }

        private void AddTypeParser(CodeNodeBuilder node, string parserName) {

            node.AddNode($"private sealed class {parserName} : TypeParser <{TypeInfo.TypeSymbol}>", node => {

                node.AddStatement($"private readonly {TypeInfo.GetDefaultParserCollectionName()} ParserCollections");

                // Figure out if this is actually fixed in size
                node.AddStatement($"private readonly int? fixedSize");
                node.AddStatement($"public override int? FixedSize => fixedSize");

                // Initialize the parser at runtime
                node.AddPlain("// For plain default namespace, useful if type is sealed");
                node.AddNode($"public {parserName} ({nameof(INamespaceRoot)} root) : this(root.CreateDiscovery())", node => { });

                node.AddPlain("// This is used for TryProvideParser pattern if not sealed");
                node.AddNode($"public {parserName} ({nameof(IParserDiscovery)} parser, out bool isSuccess)", node => AddConstructor(node, "isSuccess"));

                node.AddPlain("// This is used for ProvideParser pattern if not sealed");
                node.AddNode($"public {parserName} ({nameof(IParserDiscovery)} parser)", node => AddConstructor(node));

                node.AddPlain("");
                node.AddPlain("// Deserialization Function");
                node.AddPlain(TryDeserialize.FunctionSourceText);
                node.AddPlain("");

                node.AddPlain("// TrySerialization Function");
                node.AddPlain(TrySerialize.FunctionSourceText);
                node.AddPlain("");

                node.AddPlain("// Serialization Function");
                node.AddPlain(Serialize.FunctionSourceText);
                node.AddPlain("");

                node.AddPlain("// GetLength Function");
                node.AddPlain(GetLength.FunctionSourceText);
                node.AddPlain("");

                // Implement that abstract class
                node.AddPlain("// TryDeserialize Function");
                node.AddNode($"public override {nameof(DeserializeResult)} TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out {TypeInfo.TypeSymbol} result)", node => {
                    // If the type was never meant for deserialization, we should throw here
                    node.AddStatement($"return {TryDeserialize.FunctionName}(in ParserCollections, span, out readBytes, out result)");
                });

                node.AddPlain("");
                node.AddPlain("// GetBinaryLength Function");
                node.AddNode($"public override int GetBinaryLength({TypeInfo.TypeSymbol} value)", node => {
                    node.AddStatement($"return fixedSize ?? {GetLength.FunctionName}(in ParserCollections, value)");
                });

                node.AddPlain("");
                node.AddPlain("// TrySerialize Function");
                node.AddNode($"public override bool TrySerialize({TypeInfo.TypeSymbol} value, Span<byte> destination, out int writtenBytes)", node => {
                    node.AddStatement($"return {TrySerialize.FunctionName}(in ParserCollections, value, destination, out writtenBytes)");
                });

                node.AddPlain("");
                node.AddPlain("// Serialize Function");
                node.AddNode($"public override int Serialize({TypeInfo.TypeSymbol} value, Span<byte> destination)", node => {
                    node.AddStatement($"return {Serialize.FunctionName}(in ParserCollections, value, destination)");
                });

            });

        }

        public GeneratedParser GetFormatterCode() {
            var parserName = $"{TypeInfo.TypeSymbol.GetEmbedName()}_TypeParser";

            var parserCollectionNode = new CodeNodeBuilder();
            AddParserCollection(parserCollectionNode);
            var parserCollectionCode = new EmbeddedCode {
                CodeNamespaces = Array.Empty<string>(),
                SourceCode = parserCollectionNode.ToString(),
            };

            var parserTypeNode = new CodeNodeBuilder();
            AddTypeParser(parserTypeNode, parserName);
            var parserTypeCode = new EmbeddedCode {
                CodeNamespaces = Array.Empty<string>(),
                SourceCode = parserTypeNode.ToString(),
            };
            var parserKind = GetProviderFromType(parserName, ref parserTypeCode);

            return new GeneratedParser {
                ParserTypeName = TypeInfo.TypeSymbol.Name,
                AdditionalSourceFiles = AdditionalSourceCode,
                ParserNamespaces = TypeInfo.FormatInfo.TargetNamespaces,
                ParserKinds = new IParserKindProvider[] { parserKind },
                EmbeddedCode = parserCollectionCode.Merge(parserTypeCode),
            };
        }
    }
}
