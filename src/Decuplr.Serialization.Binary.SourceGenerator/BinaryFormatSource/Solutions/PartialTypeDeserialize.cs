﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.SourceGenerator.BinaryFormatSource;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator {

    internal class PartialTypeDeserialize : IDeserializeSolution {

        private INamedTypeSymbol TypeSymbol => TypeInfo.Type.TypeSymbol;
        private IReadOnlyList<MemberFormatInfo> Member => TypeInfo.Member;
        private readonly TypeFormatLayout TypeInfo;

        public PartialTypeDeserialize(TypeFormatLayout typeInfo) {
            TypeInfo = typeInfo;
        }

        public GeneratedSourceCode[] GetAdditionalFiles() { 
             return new GeneratedSourceCode[] { ($"{TypeSymbol.Name}.Generated.PartialDeserialize.cs", CreatePartialClassConstructor()) };
        }

        public GeneratedFormatFunction GetDeserializeFunction() {
            var builder = new CodeNodeBuilder();
            // Standard function name for invocation is "DeserializeResult TryCreateType(in ParserCollection parsers, ReadOnlySpan<byte> span, out int readBytes, out Symbol result)"
            builder.AddNode($"private static {nameof(DeserializeResult)} TryCreateType(in {TypeInfo.GetDefaultParserCollectionName()} parsers, ReadOnlySpan<byte> span, out int readBytes, out {TypeSymbol} result)", node => {
                node.AddStatement($"result = new {TypeSymbol} (parsers, span, out readBytes, out var deserializeResult);");
                node.AddStatement($"return deserializeResult");
            });
            return new GeneratedFormatFunction("CreateType", builder.ToString());
        }

        private string CreatePartialClassConstructor() {
            
            var builder = new CodeSnippetBuilder(TypeSymbol.ContainingNamespace.ToString());
            builder.Using("System");
            builder.Using("System.ComponentModel");
            builder.Using("System.CodeDom.Compiler");
            builder.Using("Decuplr.Serialization");
            builder.Using("Decuplr.Serialization.Binary");
            builder.Using($"static Decuplr.Serialization.Binary.Internal.{TypeSymbol.ContainingAssembly.GetDefaultAssemblyEntryClass()}");

            builder.AddPlain("// This file is automatically generated by Decuplr.Serilization.Binary library");
            builder.AddPlain("// For more information, see https://decuplr.dev/serialization/binary");
            builder.AddPlain("");
            builder.AddPlain($"// Debug Info : Built by {nameof(PartialTypeDeserialize)}");

            // {public} {partial} {class/ struct} Name {
            builder.AddPartialClass(TypeSymbol, node => {

                node.AddAttribute(CommonAttributes.GeneratedCodeAttribute);
                node.AddAttribute(CommonAttributes.HideFromEditor);

                // This is used for non "TryDeserialize" version
                // Function Signature 
                // internal {Type} (in ParserCollection parsers, ReadOnlySpan<byte> span, out DeserializeResult result, out int readBytes) {
                //    
                // }
                //
                node.AddPlain("// This is for \"TryDeserialize\" constructor");
                node.AddNode($"internal {TypeSymbol.Name} (in {TypeInfo.GetDefaultParserCollectionName()} parsers, ReadOnlySpan<byte> span, out int readBytes, out {nameof(DeserializeResult)} result) {(TypeSymbol.TypeKind == TypeKind.Struct ? ":this()" : null)}", node => {
                    
                    node.AddStatement("readBytes = -1");
                    node.AddStatement("var originalSpanLength = span.Length");

                    for (var i = 0; i < Member.Count; ++i)
                        node.AddNode(CreateMemberNode(i));

                    node.AddStatement("readBytes = originalSpanLength - span.Length");
                    // errr can be emitted?
                    node.AddStatement($"result = {nameof(DeserializeResult)}.Success");
                });

                // In case this class has a default constructor we need to implicit specifiy it for it
                if (TypeSymbol.Constructors.Any(member => member.Parameters.IsDefaultOrEmpty) && TypeSymbol.TypeKind != TypeKind.Struct)
                    node.AddNode($"public {TypeSymbol.Name} () ", node => { });
            });
            return builder.ToString();
        }

        private Action<CodeNodeBuilder> CreateMemberNode(int current) => node => {
            var symbol = Member[current].Symbol;
            node.AddPlain($"// Configure for argument {symbol.Name}");
            node.AddStatement("var currentReadBytes = 0");
            // Checks if the field is declared to have a fixed binary length
            if (Member[current].ConstantLength.HasValue) {
                node.AddStatement($"var localSpan = span.Slice(0, {Member[current].ConstantLength!.Value})");
                node.AddStatement($"result = TryDeserialize_{symbol.Name} (ReadOnlySpan<byte> providedSpan, out currentReadBytes, out {symbol.Name})");
                // These two check if the result is either insufficient of faulted.
                // Since this field is required to have a solid size, we rewrites InsufficientSize to "Faulted"
                node.AddNode($"if (result  == DeserializeResult.{nameof(DeserializeResult.InsufficientSize)})", node => {
                    node.AddStatement($"result = DeserializeResult.Faulted(\"Expected {symbol.Name} in {TypeSymbol} to have a constant size of {Member[current].ConstantLength!.Value} but was insufficient.\")");
                    node.AddStatement($"return");
                });
                node.AddNode($"if (result == DeserializeResult.{nameof(DeserializeResult.Faulted)}", node => {
                    node.AddStatement("return");
                });
            }
            else {
                // Here the field is not declared to have a fixed binary length, thus we only return faulty results back
                node.AddStatement($"result = TryDeserialize_{symbol.Name} (ReadOnlySpan<byte> providedSpan, out currentReadBytes, out {symbol.Name})");
                node.AddNode($"if (result != DeserializeResult.{nameof(DeserializeResult.Success)})", node => {
                    node.AddStatement("return");
                });
            }
            // After completing parsing the data we slice the span for next persons consumption
            node.AddStatement("span = span.Slice(currentReadBytes)");

            // Local function for the member to actual deserializing work
            node.AddAttribute(CommonAttributes.Inline);
            node.AddNode($"DeserializeResult TryDeserialize_{symbol.Name} (ReadOnlySpan<byte> providedSpan, out int readBytes, out {Member[current].TypeSymbol} value)", node => {
                switch (Member[current].DecisionAnnotation) {
                    case BitUnionAnnotation bitUnion:

                        break;
                    case FormatAsAnnotation formatAs:

                        break;
                    default:
                        node.AddStatement($"return parsers.Parser_{current}_0.TryDeserialize(providedSpan, out readBytes, out value)");
                        break;
                }
            });
        };
    }

}
