using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Decuplr.Serialization.Analyzer.BinaryFormat;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    class TypeParserGenerator {

        private readonly TypeFormatLayout TypeInfo;
        private readonly GeneratedFormatFunction DeserializeInfo;
        private readonly GeneratedFormatFunction SerializeInfo;
        private readonly List<GeneratedSourceCode> AdditionalSourceCode = new List<GeneratedSourceCode>();

        public IReadOnlyList<GeneratedSourceCode> AdditionalCode => AdditionalSourceCode;

        public TypeParserGenerator(TypeFormatLayout typeInfo, IDeserializeSolution deserializeSolution, ISerializeSolution serializeSolution) {
            TypeInfo = typeInfo;
            
            DeserializeInfo = deserializeSolution.GetDeserializeFunction();
            AdditionalSourceCode.AddRange(deserializeSolution.GetAdditionalFiles());

            SerializeInfo = serializeSolution.GetSerializeFunction();
            AdditionalSourceCode.AddRange(serializeSolution.GetAdditionalFiles());
        }


        public GeneratedParser GetFormatterCode() {
            var parserName = $"{TypeInfo.TypeSymbol.ToString().Replace('.', '_') }_TypeParser";

            var node = new CodeNodeBuilder();
            node.AddNode($"private class {parserName} : TypeParser <{TypeInfo.TypeSymbol}>", node => {

                // Declare the parser we will be using
                for (var i = 0; i < TypeInfo.Member.Count; ++i) {
                    for (var j = 0; j < TypeInfo.Member[i].DecisionAnnotation.RequestParserType.Count; ++j) {
                        node.AddStatement($"private readonly TypeParser<{TypeInfo.Member[i].TypeSymbol}> parser_{i}_{j}");
                    }
                }

                // Figure out if this is actually fixed in size
                node.AddStatement($"private readonly int? fixedSize");
                node.AddStatement($"public override int? FixedSize => fixedSize");

                // Initialize the parser at runtime
                // Note : we don't need to optimize so that we can inline IL or code to startup (I think the benefit is slim)
                node.AddNode($"public {parserName} ({nameof(IParserDiscovery)} parser, out bool isSuccess)", node => {
                    node.AddStatement("isSuccess = false");
                    node.AddStatement("fixedSize = 0");
                    for (var i = 0; i < TypeInfo.Member.Count; ++i) {
                        var annotation = TypeInfo.Member[i].DecisionAnnotation;
                        var currentNamespace = TypeInfo.Member[i].UsedNamespaces;
                        node.AddStatement($"var parserSpace_{i} = parser.WithNamespace({string.Join(",", currentNamespace)})");
                        for (var j = 0; j < annotation.RequestParserType.Count; ++j) {

                            node.AddNode($"if (!parserSpace_{i}.TryGetParser(out parser_{i}_{j})", node => {
                                node.AddStatement("return");
                            });

                            // force capture of i
                            var captureI = i;
                            node.AddNode("if (fixedSize != null)", node => {
                                node.AddStatement($"fixedSize += parser_{i}_{j}.FixedSize");
                            });
                        }
                    }
                    node.AddStatement("isSuccess = true");
                });

                node.AddPlain("");
                node.AddPlain("// Deserialization Function");
                node.AddPlain(DeserializeInfo.FunctionSourceText);
                node.AddPlain("");

                node.AddPlain("// Serialization Function");
                node.AddPlain(SerializeInfo.FunctionSourceText);
                node.AddPlain("");

                // Implement that abstract class
                node.AddPlain("// TryDeserialize Function");
                Add_TryDeserialize(node);

                node.AddPlain("");
                node.AddPlain("// GetBinaryLength Function");
                Add_GetBinaryLength(node);

                node.AddPlain("");
                node.AddPlain("// Serialize Function");
                Add_Serialize(node);

            });

            return new GeneratedParser {
                ParserClassName = parserName,
                ParserSourceText = node.ToString()
            };
        }

        private void Add_TryDeserialize(CodeNodeBuilder node) {
            // Implement TrySerialize(span, readBytes, result)
            node.AddNode($"public override {nameof(DeserializeResult)} TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out {TypeInfo.TypeSymbol} result)", node => {
                // In case we fail
                node.AddStatement("readBytes = 0");
                node.AddStatement("result = default");

                // Try to serialize every member
                for (var i = 0; i < TypeInfo.MemberFormatInfo.Count; ++i) {
                    // force capture of i
                    var locali = i;
                    node.AddStatement($"{TypeInfo.MemberFormatInfo[i].MemberTypeSymbol} s_{i}");
                    node.AddNode(node => {
                        node.AddStatement($"var parserResult = parser_{locali}.TryDeserialize(span, out var scopeReadBytes, out s_{locali})");
                        node.AddNode($"if (parserResult != DeserializeResult.Success)", node => {
                            node.AddStatement("readBytes = -1");
                            node.AddStatement("return parserResult");
                        });
                        node.AddStatement($"span = span.Slice(readBytes)");
                        node.AddStatement("readBytes += scopeReadBytes");
                    });
                }
                // Invoke entry compose point
                // arg = s_1, s_2, s_3
                var args = string.Join(", ", Enumerable.Range(0, TypeInfo.MemberFormatInfo.Count).Select(i => $"s_{i}"));
                node.AddStatement($"result = {DeserializeInfo.FunctionName}({args})");
                node.AddStatement("return DeserializeResult.Success");
            });
        }

        private void Add_GetBinaryLength(CodeNodeBuilder node) {
            node.AddNode($"public override int GetBinaryLength({TypeInfo.TypeSymbol} value)", node => {
                // We simply return it if it's fixed size
                node.AddNode("if (fixedSize.HasValue)", node => {
                    node.AddStatement("return fixedSize.Value");
                });

                // out var s_1, out var s_2
                var outArgs = string.Join(",", Enumerable.Range(0, TypeInfo.MemberFormatInfo.Count).Select(i => $"out var s_{i}"));
                node.AddStatement($"{SerializeInfo.FunctionName} (value, {outArgs})");

                node.AddStatement($"int length = 0");
                for (var i = 0; i < TypeInfo.MemberFormatInfo.Count; ++i) {
                    node.AddStatement($"length += parser_{i}.GetBinaryLength(s_{i})");
                }
                node.AddStatement("return length");
            });
        }

        private void Add_Serialize(CodeNodeBuilder node) {
            node.AddNode($"public override bool TrySerialize({TypeInfo.TypeSymbol} value, Span<byte> destination, out int writtenBytes)", node => {

                // Initialize out parameters
                node.AddStatement("writtenBytes = -1");

                // We simply return it if it's fixed size
                node.AddNode("if (fixedSize.HasValue && destination.Length < fixedSize)", node => {
                    node.AddStatement("return false");
                });

                // out var s_1, out var s_2
                var outArgs = string.Join(",", Enumerable.Range(0, TypeInfo.MemberFormatInfo.Count).Select(i => $"out var s_{i}"));
                node.AddStatement($"{SerializeInfo.FunctionName} (value, {outArgs})");

                node.AddStatement($"int length = 0");
                for (var i = 0; i < TypeInfo.MemberFormatInfo.Count; ++i) {
                    // force value capture
                    var localI = i;
                    node.AddNode(node => {
                        node.AddNode($"if (!parser_{localI}.TrySerialize(s_{localI}, destination, out int bytes)) ", node => {
                            node.AddStatement("return false");
                        });
                        node.AddStatement("length += bytes");
                        node.AddStatement("destination = destination.Slice(bytes)");
                    });
                }
                node.AddStatement("writtenBytes = length");
                node.AddStatement("return true");
            });
        }
    }
}
