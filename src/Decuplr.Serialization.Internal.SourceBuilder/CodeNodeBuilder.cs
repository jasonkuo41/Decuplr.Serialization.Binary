using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Linq;

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

        public void AddNode(Accessibility accessibility, string nodename, Action<CodeNodeBuilder> builder) {
            AddNode($"{accessibility.ToString().ToLower()} {nodename}", builder);
        }

        public void AddNode(string nodename, Action<CodeNodeBuilder> builder) {
            _layout.Add(new NodeInfo(nodename, builder));
        }

        public void AddNode(Action<CodeNodeBuilder> builder) {
            AddNode("", builder);
        }

        public void AddAttribute(string attribute) {
            if (string.IsNullOrWhiteSpace(attribute))
                return;
            if (attribute.AnyClampsWith("[","]"))
                AddPlain(attribute);
            else
                AddPlain($"[{attribute}]");
        }

        public void AddStatement(string statement) {
            if (string.IsNullOrWhiteSpace(statement))
                return;
            if (statement.AnyEndsWith(";"))
                AddPlain(statement);
            else
                AddPlain($"{statement};");
        }

        public void AddPlain(string plain) => _layout.Add(plain);

        public void AddLine() => AddPlain(string.Empty);

        private protected IndentedStringBuilder ToString(IndentedStringBuilder builder) {
            foreach(var layout in _layout) {
                if (layout is string str)
                    builder.AppendLine(str);
                if (layout is NodeInfo nodeInfo) {
                    var subnode = new CodeNodeBuilder();
                    nodeInfo.NodeAction(subnode);

                    builder.Append(nodeInfo.Name);
                    builder.AppendLine("{");
                    builder = subnode.ToString(builder.NextIndentation());
                    builder.AppendLine("}");
                }
            }
            return builder;
        }

        public override string ToString() => ToString(new IndentedStringBuilder()).ToString();
    }
}
