using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal class EntryPointBuilder {

        public static string CreateSourceText(IEnumerable<TypeAnalyzer> typeAnalyzer) {
            var builder = new CodeSnippetBuilder("Decuplr.Serialization.Binary.Internal");
            builder.Using("System");
            builder.Using("System.ComponentModel");
            builder.Using("System.CodeDom.Compiler");

            builder.AddAttribute($"[BinarySerializerAssemblyEntryPoint]");
            builder.AddAttribute($"[GeneratedCode (\"{Assembly.GetExecutingAssembly().GetName().Name}\", \"{Assembly.GetExecutingAssembly().GetName().Version}\")]");
            builder.AddAttribute("[EditorBrowsable(EditorBrowsableState.Never)]");
            builder.AddNode($"public class BinarySerializer_{Guid.NewGuid().ToString().Replace('-', '_')}", node => {
                node.AddNode($"public static void AppendSerializers(BinarySerializer serializer)", node => {
                    // They are pretty much global and wouldn't use namespace
                    foreach (var type in typeAnalyzer)
                        node.AddStatement($"serializer.AddSerializer(new {type.GeneratedSerializerFullName}())");
                });
            });

            return builder.ToString();
        }
    }
}
