using System;
using System.CodeDom.Compiler;
using System.Text;

namespace Decuplr.Serialization.SourceBuilder {
    internal class IndentedStringBuilder {
        private readonly int _tabCount;
        private readonly StringBuilder _builder;

        public IndentedStringBuilder() {
            _builder = new StringBuilder();
        }

        private IndentedStringBuilder(int tabCount, StringBuilder builder) {
            _tabCount = tabCount;
            _builder = builder;
        }

        public IndentedStringBuilder NextIndentation() => new IndentedStringBuilder(_tabCount + 1, _builder);

        public IndentedStringBuilder Append(char c) {
            _builder.Append(c);
            return this;
        }

        public IndentedStringBuilder Append(string str) {
            _builder.Append(str);
            return this;
        }

        public IndentedStringBuilder AppendLine(string str) {
            for(var i = 0; i < _tabCount; ++i) {
                _builder.Append('\t');
            }
            _builder.Append(str);
            return this;
        }

        public IndentedStringBuilder AppendLine() {
            _builder.AppendLine();
            return this;
        }

        public override string ToString() => _builder.ToString();
    }
}