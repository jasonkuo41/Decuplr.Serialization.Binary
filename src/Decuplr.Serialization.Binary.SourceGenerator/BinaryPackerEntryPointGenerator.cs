using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal class BinaryPackerEntryPointGenerator {

        private struct EmbeddedClass {
            public string ClassSourceText { get; set; }
            public string FunctionSourceText { get; set; }
        }

        private enum ParserType {
            Sealed,
            ParserProvider,
            GenericParserProvider
        }

        public static GeneratedSourceCode CreateSourceText(Compilation compilation, IEnumerable<GeneratedParser> parsers) {
            var generatedClassName = compilation.Assembly.GetDefaultAssemblyEntryClass();

            var builder = new CodeSnippetBuilder("Decuplr.Serialization.Binary.Internal");
            builder.Using("System");
            builder.Using("System.Threading");
            builder.Using("System.ComponentModel");
            builder.Using("System.CodeDom.Compiler");

            builder.AddAssemblyAttribute($"[assembly: {nameof(BinaryPackerAssemblyEntryPointAttribute)}(typeof({generatedClassName}))]");

            builder.AddAttribute(CommonAttributes.GeneratedCodeAttribute);
            builder.AddAttribute(CommonAttributes.HideFromEditor);
            builder.AddNode($"internal class {generatedClassName} : {nameof(AssemblyPackerEntryPoint)} ", node => {

                var namespaceName = "defaultNamespace";
                // Put all the generated formatters class here
                var finalParser = TransformParserToConsumable(parsers, namespaceName).ToList();
                foreach (var parser in finalParser)
                    node.AddPlain(parser.ClassSourceText);

                // Define the entry function
                node.AddNode($"public override void {nameof(AssemblyPackerEntryPoint.LoadContext)} ({nameof(INamespaceRoot)} root)", node => {
                    node.AddStatement($"var {namespaceName} = root.DefaultNamespace");
                    foreach (var parser in finalParser) {
                        node.AddStatement(parser.FunctionSourceText);
                    }
                });
            });

            return ($"{generatedClassName}.cs", builder.ToString());
        }

        private static IEnumerable<EmbeddedClass> TransformParserToConsumable(IEnumerable<GeneratedParser> parsers, string nsName) {
            foreach (var parser in parsers) {
                var formatInfo = parser.TypeInfo.FormatInfo;
                if (!formatInfo.HasValue)
                    continue;
                // Check if it's a sealed parser
                if (formatInfo.Value.IsSealed)
                    yield return new EmbeddedClass {
                        ClassSourceText = parser.ParserSourceText,
                        FunctionSourceText = $"{nsName}.{nameof(IDefaultParserNamespace.AddParserProvider)}(new {parser.ParserClassName}(root))"
                    };
                // Check if it's a generic type 
                if (parser.TypeInfo.TypeSymbol.IsUnboundGenericType)
                    yield return WrapAsGenericParserProvider(nsName, parser);
                yield return WrapAsParserProvider(nsName, parser);
            }
        }

        // Returns the class name
        private static EmbeddedClass WrapAsGenericParserProvider(string nsName, GeneratedParser parser) {
            var parserProviderName = $"{parser.ParserClassName}_GenericProvider";
            var genParam = parser.TypeInfo.TypeSymbol.TypeParameters;
            var node = new CodeNodeBuilder();
            // TODO : Please make sure this works, I have no doc over this!! (or at least it's undoced)
            node.AddNode($"private class {parserProviderName}<{string.Join(",", genParam.Select(x => x.Name))}> : GenericParserProvider", node => {
                // Embed the source code in this class
                node.AddPlain(parser.ParserSourceText);

                node.AddNode("public override TypeParser ProviderParser(IParserDiscovery discovery)", node => {
                    node.AddStatement($"return new {parser.ParserClassName}(discovery)");
                });

                node.AddNode("public override bool TryProvideParser(IParserDiscovery discovery, out TypeParser parser)", node => {
                    // Code Review :
                    // Duplicate code as below, but eliminating this might lead to confusion, since they actually respresent different meaning
                    // Just the output code is the same, the compiler would understand that it's different context
                    //
                    node.AddStatement($"parser = new {parser.ParserClassName}(discovery, out var isSuccess)");
                    node.AddNode("if(!isSuccess)", node => {
                        // we don't want a half baked parser to be returned and possibly used
                        node.AddStatement("parser = null");
                    });
                    node.AddStatement($"return isSuccess");
                });
            });

            return new EmbeddedClass {
                ClassSourceText = node.ToString(),
                // bool AddGenericParserProvider(Type parserProvider, Type genericType) where TParser : GenericParserProvider;
                FunctionSourceText = $"{nsName}.{nameof(IDefaultParserNamespace.AddGenericParserProvider)}({parserProviderName}, typeof({parser.TypeInfo.TypeSymbol}).GetGenericTypeDefinition())"
            };
        }

        private static EmbeddedClass WrapAsParserProvider(string nsName, GeneratedParser parser) {
            var parserProviderName = $"{parser.ParserClassName}_Provider";

            var node = new CodeNodeBuilder();
            
            // Add the parser to our source, not nested but outside
            node.AddPlain(parser.ParserSourceText);

            node.AddNode($"private class {parserProviderName} : IParserProvider<{parser.TypeInfo.TypeSymbol}>", node => {
                node.AddNode($"public TypeParser<{parser.TypeInfo.TypeSymbol}> ProvideParser(IParserDiscovery discovery)", node => {
                    node.AddStatement($"return new {parser.ParserClassName}(discovery)");
                });

                node.AddNode($"public bool TryProvideParser(IParserDiscovery discovery, out TypeParser<{parser.TypeInfo.TypeSymbol}> parser)", node => {
                    node.AddStatement($"parser = new {parser.ParserClassName}(discovery, out var isSuccess)");
                    node.AddNode("if(!isSuccess)", node => {
                        // we don't want a half baked parser to be returned and possibly used
                        node.AddStatement("parser = null");
                    });
                    node.AddStatement($"return isSuccess");
                });
            });

            return new EmbeddedClass {
                ClassSourceText = node.ToString(),
                // bool AddParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType>;
                FunctionSourceText = $"{nsName}.AddPraserProvider<{parserProviderName}, {parser.TypeInfo.TypeSymbol}(new {parserProviderName}())"
            };
        }
    }
}
