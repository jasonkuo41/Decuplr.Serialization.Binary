using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Decuplr.Serialization.Binary.Internal;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal class EntryPointBuilder {

        public static string CreateSourceText(IReadOnlyList<FormatterSourceCode> formatters) {
            var builder = new CodeSnippetBuilder("Decuplr.Serialization.Binary.Internal");
            builder.Using("System");
            builder.Using("System.Threading");
            builder.Using("System.ComponentModel");
            builder.Using("System.CodeDom.Compiler");

            builder.AddAttribute($"[{nameof(BinaryFormatterAssemblyEntryPointAttribute)}]");
            builder.AddAttribute($"[GeneratedCode (\"{Assembly.GetExecutingAssembly().GetName().Name}\", \"{Assembly.GetExecutingAssembly().GetName().Version}\")]");
            builder.AddAttribute("[EditorBrowsable(EditorBrowsableState.Never)]");
            builder.AddNode($"public class AssemblyFormatProvider_{Guid.NewGuid().ToString().Replace('-', '_')}", node => {

                // Ensure TS
                node.AddStatement("private static int IsInit = 0");

                // Put all the generated formatters class here
                foreach (var formatter in formatters) {
                    node.AddPlain("");
                    node.AddPlain(formatter.FormatterClassSourceText);
                    node.AddPlain("");
                }

                // Define the entry function
                node.AddNode($"public static void {BinaryFormatterAssemblyEntryPointAttribute.EntryFunctionName} ({nameof(BinaryFormatter)} formatter)", node => {

                    node.AddPlain("// This ensures that we only invoke only once (ThreadSafe)");

                    node.AddNode("if (Interlocked.Exchange(ref IsInit, 1) != 0)", node => {
                        node.AddStatement("return");
                    });
                    foreach (var formatter in formatters)
                        node.AddStatement($"formatter.AddFormatter(new {formatter.FormatterName}(formatter))");
                });
            });

            return builder.ToString();
        }
    }
}
