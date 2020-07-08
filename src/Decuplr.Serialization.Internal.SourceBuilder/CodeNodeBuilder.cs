using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Decuplr.Serialization.SourceBuilder {
    public class CodeNodeBuilder {

        private class NodeInfo {
            public string Name { get; set; }
            public Action<CodeNodeBuilder> NodeAction { get; set; }
        }

        private readonly List<object> Layout = new List<object>();

        public void AddNode(Accessibility accessibility, string nodename, Action<CodeNodeBuilder> builder) {
            AddNode($"{accessibility.ToString().ToLower()} {nodename}", builder);
        }

        public void AddNode(string nodename, Action<CodeNodeBuilder> builder) {
            Layout.Add(new NodeInfo { Name = nodename, NodeAction = builder });
        }

        public void AddNode(Action<CodeNodeBuilder> builder) {
            AddNode("", builder);
        }

        public void AddAttribute(string attribute) {
            attribute = attribute.Trim();
            if (attribute.ClampsWith("[","]"))
                AddPlain(attribute);
            else if (string.IsNullOrWhiteSpace(attribute))
                return;
            else
                AddPlain($"[{attribute}]");
        }

        public void AddStatement(string statement) {
            statement = statement.Trim();
            if (statement.EndsWith(";"))
                AddPlain(statement);
            else if (string.IsNullOrWhiteSpace(statement))
                return;
            else
                AddPlain($"{statement};");
        }

        public void AddPlain(string plain) => Layout.Add(plain);
        public void AddLine() => AddPlain(string.Empty);

        protected StringBuilder ToString(StringBuilder builder) {
            foreach(var layout in Layout) {
                if (layout is string str)
                    builder.AppendLine(str);
                if (layout is NodeInfo nodeInfo) {
                    var subnode = new CodeNodeBuilder();
                    nodeInfo.NodeAction(subnode);

                    builder.Append(nodeInfo.Name);
                    builder.AppendLine("{");
                    builder = subnode.ToString(builder);
                    builder.AppendLine("}");
                }
            }
            return builder;
        }

        public override string ToString() => ToString(new StringBuilder()).ToString();
    }
}
