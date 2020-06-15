using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary {

    internal class BinaryPackerEntryPointGenerator {

        public static GeneratedSourceCode CreateSourceText(Compilation compilation, IEnumerable<GeneratedParser> parsers) {
            var generatedClassName = compilation.GetDefaultAssemblyEntryClass();

            CodeSnippetBuilder? builder = new CodeSnippetBuilder("Decuplr.Serialization.Binary.Internal");
            builder.Using("System");
            builder.Using("System.Collections.Generic");
            builder.Using("System.ComponentModel");
            builder.Using("System.CodeDom.Compiler");
            builder.Using("Decuplr.Serialization.Binary.Internal");
            builder.Using("Decuplr.Serialization.Binary.Namespaces");
            foreach (var useNamespace in parsers.SelectMany(x => x.EmbeddedCode.CodeNamespaces ?? Enumerable.Empty<string>())) {
                builder.Using(useNamespace);
            }

            // If we are default parser we don't need this
            if (!compilation.IsDefaultAssembly())
                builder.AddAssemblyAttribute($"[assembly: {nameof(BinaryPackerAssemblyEntryPointAttribute)}(typeof({generatedClassName}))]");

            builder.AddAttribute(CommonAttributes.GeneratedCodeAttribute);
            builder.AddAttribute(CommonAttributes.HideFromEditor);
            builder.AddNode($"internal partial class {generatedClassName} : {nameof(AssemblyPackerEntryPoint)} ", node => {

                // Put all the generated parser's embedded class here
                foreach (GeneratedParser parser in parsers) {
                    node.AddPlain("");
                    node.AddPlain($"#region {parser.ParserTypeName}");
                    node.AddPlain(parser.EmbeddedCode.SourceCode);
                    node.AddPlain($"#endregion");
                }

                // Define the entry function
                node.AddNode($"public override void {nameof(AssemblyPackerEntryPoint.LoadContext)} ({nameof(INamespaceRoot)} root)", node => {
                    node.AddStatement($"var namespaces = new Dictionary<string, {nameof(IMutableNamespace)}>()");
                    // Set reference to default namespace
                    var defaultSet = new HashSet<string> { string.Empty, "default", "Default", "DEFAULT" };
                    foreach (var defaultNamespaces in defaultSet)
                        node.AddStatement($"namespaces[\"{defaultNamespaces}\"] = root.{nameof(INamespaceRoot.DefaultNamespace)}");

                    // Add custom namespace
                    foreach (var parserNamespace in parsers.SelectMany(x => x.ParserNamespaces).Where(x => !defaultSet.Contains(x)).Distinct())
                        node.AddStatement($"namespaces[\"{parserNamespace}\"] = root.CreateNamespace({parserNamespace})");

                    node.AddPlain($"// Generated Count : {parsers.Count()}");
                    foreach (var (parserNamespaces, parserKinds) in parsers.Select(x => (x.ParserNamespaces, x.ParserKinds))) {
                        foreach (var parserNamespace in parserNamespaces) {
                            foreach (var parserKind in parserKinds) {
                                node.AddStatement($"namespaces[\"{parserNamespace}\"].{parserKind.GetFunction("root")}");
                            }
                        }
                    }
                });
            });

            return ($"{generatedClassName}.Generated.cs", builder.ToString());
        }

    }
}
