using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    class CodeSnippetBuilder : CodeNodeBuilder {

        private readonly string ThisNamespace;
        private readonly List<string> Namespaces = new List<string>();

        public CodeSnippetBuilder(string thisNamespace) {
            ThisNamespace = thisNamespace;
        }

        public void Using(string namespaceName) {
            Namespaces.Add(namespaceName);
        }

        public override string ToString() {
            var builder = new StringBuilder();
            foreach (var namespaces in Namespaces)
                builder.AppendLine($"using {namespaces};");

            builder.AppendLine();
            builder.AppendLine($"namespace {ThisNamespace} {{");
            builder = ToString(builder);
            builder.AppendLine("}");

            return builder.ToString();
        }
    }
}
