using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel;
using System.Reflection;

namespace Decuplr.Serialization.SourceBuilder {
    public class CodeNodeBuilder {

        private class NodeInfo {
            public NodeInfo(string name, Action<CodeNodeBuilder> nodeAction) {
                Name = name;
                NodeAction = nodeAction;
            }

            public string Name { get; }
            public Action<CodeNodeBuilder> NodeAction { get; }
        }

        private readonly List<object> _layout = new List<object>();

        public CodeNodeBuilder AddNode(Accessibility accessibility, string nodename, Action<CodeNodeBuilder> builder) {
            AddNode($"{accessibility.ToString().ToLower()} {nodename}", builder);
            return this;
        }

        public CodeNodeBuilder AddNode(string nodename, Action<CodeNodeBuilder> builder) {
            _layout.Add(new NodeInfo(nodename, builder));
            return this;
        }

        public CodeNodeBuilder AddNode(Action<CodeNodeBuilder> builder) {
            AddNode("", builder);
            return this;
        }

        public CodeNodeBuilder If(string condition, Action<CodeNodeBuilder> builder) {
            AddNode($"if ({condition})", builder);
            return this;
        }

        public CodeNodeBuilder Return(string statement) {
            State($"return {statement}");
            return this;
        }

        public CodeNodeBuilder Return() {
            State($"return");
            return this;
        }

        public CodeNodeBuilder Attribute(string attribute) {
            if (string.IsNullOrWhiteSpace(attribute))
                return this;
            if (attribute.AnyClampsWith("[","]"))
                Add(attribute);
            else
                Add($"[{attribute}]");
            return this;
        }

        public CodeNodeBuilder State(string statement) {
            if (string.IsNullOrWhiteSpace(statement))
                return this;
            if (statement.AnyEndsWith(";"))
                Add(statement);
            else
                Add($"{statement};");
            return this;
        }

        public CodeNodeBuilder Comment(string comment) {
            if (comment.AnyStartsWith("//"))
                Add(comment);
            else
                Add($"// {comment}");
            return this;
        }

        public CodeNodeBuilder Add(string plain) {
            using var lineReader = new StringReader(plain);
            string line;
            while ((line = lineReader.ReadLine()) != null)
                _layout.Add(line);
            return this;
        }

        public CodeNodeBuilder NewLine() {
            Add(string.Empty);
            return this;
        }

        private protected void WriteContent(IndentedStringBuilder builder) {
            foreach(var layout in _layout) {
                if (layout is string str)
                    builder.AppendLine(str);
                if (layout is NodeInfo nodeInfo) {
                    var subnode = new CodeNodeBuilder();
                    nodeInfo.NodeAction(subnode);

                    builder.AppendLine(nodeInfo.Name);
                    builder.AppendLine("{");
                    subnode.WriteContent(builder.NextIndentation());
                    builder.AppendLine("}");
                }
            }
        }

        public override string ToString() {
            var builder = new IndentedStringBuilder();
            WriteContent(builder);
            return builder.ToString();
        }
    }

    public static class CodeNodeExtensions {
        public static CodeNodeBuilder DenoteHideEditor(this CodeNodeBuilder builder) => builder.Attribute("[EditorBrowsable(EditorBrowsableState.Never)]");
        public static CodeNodeBuilder DenoteGenerated(this CodeNodeBuilder builder, Assembly assebmly) => builder.Attribute($"[GeneratedCode({assebmly.GetName().Name}, {assebmly.GetName().Version})]")
    }
}
