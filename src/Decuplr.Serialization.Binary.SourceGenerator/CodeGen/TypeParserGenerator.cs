using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    class TypeParserGenerator {

        private readonly TypeFormatInfo TypeInfo;
        private readonly GeneratedFormatFunction DeserializeInfo;
        private readonly GeneratedFormatFunction SerializeInfo;
        private readonly List<GeneratedSourceCode> AdditionalSourceCode = new List<GeneratedSourceCode>();

        public IReadOnlyList<GeneratedSourceCode> AdditionalCode => AdditionalSourceCode;

        public TypeParserGenerator(TypeFormatInfo typeInfo, IDeserializeSolution deserializeSolution, ISerializeSolution serializeSolution) {
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
                for (var i = 0; i < TypeInfo.Members.Count; ++i)
                    node.AddStatement($"private readonly TypeParser<{TypeInfo.Members[i].MemberTypeSymbol}> parser_{i}");

                // Figure out if this is actually fixed in size
                node.AddStatement($"private readonly int? fixedSize");
                node.AddStatement($"public override int? FixedSize => fixedSize");

                // Initialize the parser at runtime
                // Note : we don't need to optimize so that we can inline IL or code to startup (I think the benefit is slim)
                node.AddNode($"public {parserName} ({nameof(IParserDiscovery)} format)", node => {
                    // Locate the parser
                    node.AddStatement("fixedSize = 0");
                    for (var i = 0; i < TypeInfo.Members.Count; ++i) {
                        // TODO : Allow namespace to run
                        // What if it's not supported?
                        node.AddStatement($"format.TryGetFormatter(out parser_{i})");

                        // force capture of i
                        var captureI = i;
                        node.AddNode("if (fixedSize != null)", node => {
                            node.AddStatement($"fixedSize += parser_{captureI}.FixedSize");
                        });
                    }
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
                for (var i = 0; i < TypeInfo.Members.Count; ++i) {
                    // force capture of i
                    var locali = i;
                    node.AddStatement($"{TypeInfo.Members[i].MemberTypeSymbol} s_{i}");
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
                var args = string.Join(", ", Enumerable.Range(0, TypeInfo.Members.Count).Select(i => $"s_{i}"));
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
                var outArgs = string.Join(",", Enumerable.Range(0, TypeInfo.Members.Count).Select(i => $"out var s_{i}"));
                node.AddStatement($"{SerializeInfo.FunctionName} (value, {outArgs})");

                node.AddStatement($"int length = 0");
                for (var i = 0; i < TypeInfo.Members.Count; ++i) {
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
                var outArgs = string.Join(",", Enumerable.Range(0, TypeInfo.Members.Count).Select(i => $"out var s_{i}"));
                node.AddStatement($"{SerializeInfo.FunctionName} (value, {outArgs})");

                node.AddStatement($"int length = 0");
                for (var i = 0; i < TypeInfo.Members.Count; ++i) {
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
