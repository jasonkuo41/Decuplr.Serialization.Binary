using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.ConditionResolver {
    internal class IgnoreIfResolver : IConditionResolver {

        private readonly MemberMetaInfo _member;
        private readonly Condition _condition;
        private readonly IConditionAnalyzer _analyzer;

        private readonly bool _isInverted;

        private NamedTypeMetaInfo Type => _member.ContainingFullType;

        public string FormatterName => "IgnoreIf";

        public IgnoreIfResolver(MemberMetaInfo member, Condition condition, IConditionAnalyzer analyzer, bool isInverted) {
            _member = member;
            _condition = condition;
            _analyzer = analyzer;
            _isInverted = isInverted;
        }

        private string MethodBase(TypeSourceArgs source, string nextMethodInvoke) {
            var node = new CodeNodeBuilder();

            node.If($"{(_isInverted ? "!" : "")}({_analyzer.GetEvalString(source.ToString(), Type, _condition)})", node => {
                node.Return(nextMethodInvoke);
            });
            node.Return(DeserializeResult.Success.ToDisplayString());

            return node.ToString();
        }

        public string GetMethodBody(string? nextMethodName, TryDeserializeSpanArgs<TypeSourceArgs> args) 
            => MethodBase(args.Source, $"{nextMethodName}({args.Source}, {args.ReadOnlySpan}, out {args.OutReadBytes}, out {args.OutResult})");

        public string GetMethodBody(string? nextMethodName, TryDeserializeSequenceArgs<TypeSourceArgs> args)
            => MethodBase(args.Source, $"{nextMethodName}({args.Source}, ref {args.RefSequenceCursor}, out {args.OutResult})");

        public string GetMethodBody(string? nextMethodName, DeserializeSpanArgs<TypeSourceArgs> args)
            => MethodBase(args.Source, $"{nextMethodName}({args.Source}, {args.ReadOnlySpan}, out {args.OutReadBytes})");

        public string GetMethodBody(string? nextMethodName, DeserializeSequenceArgs<TypeSourceArgs> args)
            => MethodBase(args.Source, $"{nextMethodName}({args.Source}, ref {args.RefSequenceCursor})");

        public string GetMethodBody(string? nextMethodName, SerializeArgs<TypeSourceArgs> args)
            => MethodBase(args.Source, $"{nextMethodName}({args.Source}, {args.Target}, {args.ReadOnlySpan})");

        public string GetMethodBody(string? nextMethodName, TrySerializeArgs<TypeSourceArgs> args)
            => MethodBase(args.Source, $"{nextMethodName}({args.Source}, {args.Target}, {args.ReadOnlySpan}, out {args.OutWrittenBytes})");

        public string GetMethodBody(string? nextMethodName, GetLengthArgs<TypeSourceArgs> args)
            => MethodBase(args.Source, $"{nextMethodName}({args.Source}, {args.Target})");
    }
}
