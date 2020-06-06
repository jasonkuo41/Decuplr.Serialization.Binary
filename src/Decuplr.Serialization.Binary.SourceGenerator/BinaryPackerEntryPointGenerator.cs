using System.Collections.Generic;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;
using Decuplr.Serialization.Binary.SourceGenerator.Schemas;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator {

    internal class BinaryPackerEntryPointGenerator {

        public static GeneratedSourceCode CreateSourceText(Compilation compilation, IEnumerable<GeneratedParser> parsers) {
            var generatedClassName = compilation.GetDefaultAssemblyEntryClass();

            CodeSnippetBuilder? builder = new CodeSnippetBuilder("Decuplr.Serialization.Binary.Internal");
            builder.Using("System");
            builder.Using("System.Threading");
            builder.Using("System.ComponentModel");
            builder.Using("System.CodeDom.Compiler");
            builder.Using("Decuplr.Serialization.Binary.Internal");
            builder.Using("Decuplr.Serialization.Binary.Namespaces");

            // If we are default parser we don't need this
            if (!compilation.IsDefaultAssembly())
                builder.AddAssemblyAttribute($"[assembly: {nameof(BinaryPackerAssemblyEntryPointAttribute)}(typeof({generatedClassName}))]");

            builder.AddAttribute(CommonAttributes.GeneratedCodeAttribute);
            builder.AddAttribute(CommonAttributes.HideFromEditor);
            builder.AddNode($"internal partial class {generatedClassName} : {nameof(AssemblyPackerEntryPoint)} ", node => {

                string? namespaceName = "defaultNamespace";
                // Put all the generated formatters class here
                foreach (GeneratedParser parser in parsers) {
                    node.AddPlain("");
                    node.AddPlain($"#region {parser.ParserTypeName}");
                    node.AddPlain(parser.EmbeddedCode);
                    node.AddPlain($"#endregion");
                }

                // Define the entry function
                node.AddNode($"public override void {nameof(AssemblyPackerEntryPoint.LoadContext)} ({nameof(INamespaceRoot)} root)", node => {
                    node.AddStatement($"var {namespaceName} = root.DefaultNamespace");
                    foreach (var parser in finalParser) {
                        node.AddStatement(parser.FunctionSourceText);
                    }
                });
            });

            return ($"{generatedClassName}.Generated.cs", builder.ToString());
        }

    }
}
