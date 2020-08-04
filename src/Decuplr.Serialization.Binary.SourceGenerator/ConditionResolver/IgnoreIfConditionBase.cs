using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.ConditionResolver {
    internal abstract class IgnoreIfConditionBase : IConditionResolver {

        private readonly MemberMetaInfo _member;
        private readonly Condition _condition;
        private readonly IConditionAnalyzer _analyzer;
        
        protected abstract bool IsInverted { get; }

        private NamedTypeMetaInfo Type => _member.ContainingFullType;

        public string FormatterName => "IgnoreIf";

        public IgnoreIfConditionBase(MemberMetaInfo member, Condition condition, IConditionAnalyzer analyzer) {
            _member = member;
            _condition = condition;
            _analyzer = analyzer;
        }

        private string MethodBase(TypeSourceArgs source, string nextMethodInvoke) {
            var node = new CodeNodeBuilder();

            node.If($"{(IsInverted ? "!" : "")}({_analyzer.GetEvalString(source.ToString(), Type, _condition)})", node => {
                node.Return(nextMethodInvoke);
            });
            node.Return(DeserializeResult.Success.ToDisplayString());

            return node.ToString();
        }

        public string GetMethodBody(string? nextMethodName, TryDeserializeSpanArgs<TypeSourceArgs> args) 
            => MethodBase(args.Source, $"{nextMethodName}({args.Source}, {args.ReadOnlySpan}, out {args.OutReadBytes}, out {args.OutResult})");

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
