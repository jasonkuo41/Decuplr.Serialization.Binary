using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.ConditionResolver {
    internal class IgnoreIfCondition : IConditionalFormatter {

        private readonly MemberMetaInfo _member;
        private readonly Condition _condition;

        public string FormatterName => "IgnoreIf";

        public IgnoreIfCondition(MemberMetaInfo member, Condition condition) {
            _member = member;
            _condition = condition;
        }

        public string GetMethodBody(string? nextMethodName, TryDeserializeSpanArgs<TypeSourceArgs> args) {
            var node = new CodeNodeBuilder();
            
            node.If($"{args.Source}.{_condition.SourceName}", node => {
                node.Return($"{nextMethodName}({args.Source}, {args.ReadOnlySpan}, out {args.OutReadBytes}, out {args.OutResult})");
            });
            node.Return(DeserializeResult.Success.ToDisplayString());

            return node.ToString();
        }

        public string GetMethodBody(string? nextMethodName, TryDeserializeSequenceArgs<TypeSourceArgs> args) {
            throw new NotImplementedException();
        }

        public string GetMethodBody(string? nextMethodName, DeserializeSpanArgs<TypeSourceArgs> args) {
            throw new NotImplementedException();
        }

        public string GetMethodBody(string? nextMethodName, DeserializeSequenceArgs<TypeSourceArgs> args) {
            throw new NotImplementedException();
        }

        public string GetMethodBody(string? nextMethodName, SerializeArgs<TypeSourceArgs> args) {
            throw new NotImplementedException();
        }

        public string GetMethodBody(string? nextMethodName, TrySerializeArgs<TypeSourceArgs> args) {
            throw new NotImplementedException();
        }

        public string GetMethodBody(string? nextMethodName, GetLengthArgs<TypeSourceArgs> args) {
            throw new NotImplementedException();
        }
    }
}
