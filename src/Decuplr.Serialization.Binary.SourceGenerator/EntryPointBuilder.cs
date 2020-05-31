using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal class EntryPointBuilder {

        public static GeneratedSourceCode CreateSourceText(IEnumerable<TypeParserGenerator> builder) => CreateSourceText(builder.Select(x => x.GetFormatterCode()));
        
        public static GeneratedSourceCode CreateSourceText(IEnumerable<GeneratedParser> parsers) {
            var generatedClassName = $"AssemblyFormatProvider_{Guid.NewGuid().ToString().Replace('-', '_')}";

            var builder = new CodeSnippetBuilder("Decuplr.Serialization.Binary.Internal");
            builder.Using("System");
            builder.Using("System.Threading");
            builder.Using("System.ComponentModel");
            builder.Using("System.CodeDom.Compiler");

            builder.AddAssemblyAttribute($"[assembly: {nameof(BinaryPackerAssemblyEntryPointAttribute)}(typeof({generatedClassName}))]");

            builder.AddAttribute($"[GeneratedCode (\"{Assembly.GetExecutingAssembly().GetName().Name}\", \"{Assembly.GetExecutingAssembly().GetName().Version}\")]");
            builder.AddAttribute("[EditorBrowsable(EditorBrowsableState.Never)]");
            builder.AddNode($"internal class {generatedClassName} : {nameof(AssemblyPackerEntryPoint)} ", node => {

                // Put all the generated formatters class here
                foreach (var parser in parsers) {
                    node.AddPlain(parser.ParserSourceText);
                }

                // Define the entry function
                node.AddNode($"public override void {nameof(AssemblyPackerEntryPoint.LoadContext)} ({nameof(INamespaceRoot)} root)", node => {
                    node.AddStatement("var defaultSpace = root.DefaultNamespace");
                    foreach (var parser in parsers) {
                        // bool AddSealedParser<T>(TypeParser<T> parser);
                        node.AddStatement($"defaultSpace.{nameof(IDefaultParserNamespace.AddParserProvider)}(new {parser.ParserClassName}(formatter))");

                        // bool AddGenericParserProvider<TParser>(Type genericType) where TParser : GenericParserProvider;
                        node.AddStatement($"defaultSpace.{nameof(IDefaultParserNamespace.AddGenericParserProvider)}<>()");

                        // bool AddParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType>;
                        node.AddStatement($"defaultSpace.{nameof(IDefaultParserNamespace.AddParserProvider)}<>()");
                    }
                });
            });

            return ($"{generatedClassName}.cs", builder.ToString());
        }
    }
}
