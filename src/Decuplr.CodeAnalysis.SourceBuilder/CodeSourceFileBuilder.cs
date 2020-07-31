using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.SourceBuilder {
    public class CodeSourceFileBuilder : CodeNodeBuilder {

        private readonly string _targetNamespace;
        private readonly List<string> _namespaces = new List<string>();
        private readonly List<string> _assemblyAttributes = new List<string>();

        public CodeSourceFileBuilder(string targetNamespace) {
            _targetNamespace = targetNamespace;
        }

        public void Using(string namespaceName) {
            if (string.IsNullOrEmpty(namespaceName))
                return;
            if (!namespaceName.AnyEndsWith(";"))
                namespaceName = $"{namespaceName};";
            if (!namespaceName.AnyStartsWith("using"))
                namespaceName = $"using {namespaceName}";
            _namespaces.Add(namespaceName);
        }

        public void AddAssemblyAttribute(string attributes) {
            if (string.IsNullOrEmpty(attributes))
                return;
            if (attributes.AnyClampsWith("[", "]"))
                _assemblyAttributes.Add(attributes);
            else
                _assemblyAttributes.Add($"[{attributes}]");
        }

        public void NestType(GeneratingTypeName typeName, string nodeName, Action<CodeNodeBuilder> node) {
            Action<CodeNodeBuilder> lastAction = builder => builder.AddNode(nodeName, node);
            foreach (var (parentKind, parentName) in typeName.Parents.Reverse()) {
                lastAction = builder => builder.AddNode($"partial {parentKind.ToString().ToLower()} {parentName}", lastAction);
            }
            lastAction(this);
        }

        public override string ToString() {
            var builder = new IndentedStringBuilder();
            foreach (var namespaces in _namespaces)
                builder.AppendLine(namespaces);
            builder.AppendLine();
            foreach (var attribute in _assemblyAttributes)
                builder.AppendLine(attribute);

            builder.AppendLine();
            builder.AppendLine($"namespace {_targetNamespace} {{");
            WriteContent(builder.NextIndentation());
            builder.AppendLine("}");

            return builder.ToString();
        }
    }
}
