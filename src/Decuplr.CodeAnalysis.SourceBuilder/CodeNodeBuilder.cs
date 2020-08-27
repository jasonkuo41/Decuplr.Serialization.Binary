using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Decuplr.CodeAnalysis.SourceBuilder {
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

        public CodeNodeBuilder AddMethod(MethodSignature signature, Action<CodeNodeBuilder> builder)
            => AddNode(signature.GetDeclarationString(), builder);

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
                AddPlain(attribute);
            else
                AddPlain($"[{attribute}]");
            return this;
        }

        public CodeNodeBuilder Attribute<TAttribute>() where TAttribute : Attribute, new() {
            return Attribute(typeof(TAttribute).FullName);
        }

        public CodeNodeBuilder State(string statement) {
            if (string.IsNullOrWhiteSpace(statement))
                return this;
            if (statement.AnyEndsWith(";"))
                AddPlain(statement);
            else
                AddPlain($"{statement};");
            return this;
        }

        public CodeNodeBuilder Comment(string comment) {
            if (comment.AnyStartsWith("//"))
                AddPlain(comment);
            else
                AddPlain($"// {comment}");
            return this;
        }

        public CodeNodeBuilder AddPlain(string plain) {
            using var lineReader = new StringReader(plain);
            string line;
            while ((line = lineReader.ReadLine()) != null)
                _layout.Add(line);
            return this;
        }

        public CodeNodeBuilder NewLine() {
            AddPlain(string.Empty);
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
        
        public static CodeNodeBuilder AttributeHideEditor(this CodeNodeBuilder builder) => builder.Attribute("[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]");
        
        public static CodeNodeBuilder AttributeGenerated(this CodeNodeBuilder builder, Assembly assebmly) => builder.Attribute($"[System.CodeDom.Compiler.GeneratedCode({assebmly.GetName().Name}, {assebmly.GetName().Version})]");
        
        public static CodeNodeBuilder AttributeMethodImpl(this CodeNodeBuilder builder, MethodImplOptions options) {
            var flags = Enum.GetValues(typeof(MethodImplOptions)).Cast<Enum>().Where(options.HasFlag);
            var fullFlagName = flags.Select(x => $"System.Runtime.CompilerServices.MethodImplOptions.{x}");
            return builder.Attribute($"[System.Runtime.CompilerServices.MethodImpl({string.Join(", ", fullFlagName)})]");
        }
    }
}
