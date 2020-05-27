using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Decuplr.Serialization.Binary.Internal;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal class EntryPointBuilder {

        public static GeneratedSourceCode CreateSourceText(IEnumerable<TypeParserGenerator> builder) => CreateSourceText(builder.Select(x => x.GetFormatterCode()));
        
        public static GeneratedSourceCode CreateSourceText(IEnumerable<GeneratedFormatter> formatters) {
            var generatedClassName = $"AssemblyFormatProvider_{Guid.NewGuid().ToString().Replace('-', '_')}";

            var builder = new CodeSnippetBuilder("Decuplr.Serialization.Binary.Internal");
            builder.Using("System");
            builder.Using("System.Threading");
            builder.Using("System.ComponentModel");
            builder.Using("System.CodeDom.Compiler");

            builder.AddAttribute($"[{nameof(BinaryFormatterAssemblyEntryPointAttribute)}]");
            builder.AddAttribute($"[GeneratedCode (\"{Assembly.GetExecutingAssembly().GetName().Name}\", \"{Assembly.GetExecutingAssembly().GetName().Version}\")]");
            builder.AddAttribute("[EditorBrowsable(EditorBrowsableState.Never)]");
            builder.AddNode($"public class {generatedClassName} ", node => {

                // Ensure TS
                node.AddPlain("// Declare private member");
                node.AddStatement("private static int IsInit = 0");
                node.AddLine();

                // Put all the generated formatters class here
                foreach (var formatter in formatters) {
                    node.AddPlain(formatter.FormatterClassSourceText);
                }

                // Define the entry function
                node.AddNode($"public static void {BinaryFormatterAssemblyEntryPointAttribute.EntryFunctionName} ({nameof(BinaryPacker)} formatter)", node => {

                    node.AddPlain("// This ensures that we only invoke only once (ThreadSafe)");

                    node.AddNode("if (Interlocked.Exchange(ref IsInit, 1) != 0)", node => {
                        node.AddStatement("return");
                    });
                    foreach (var formatter in formatters)
                        node.AddStatement($"formatter.AddFormatter(new {formatter.FormatterName}(formatter))");
                });
            });

            return ($"{generatedClassName}.cs", builder.ToString());
        }
    }
}
