using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.ConditionResolver {
    internal class IgnoreIfResolver : IConditionResolver {

        private readonly MemberMetaInfo _member;
        private readonly ConditionExpression _expression;
        private readonly IConditionAnalyzer _condition;

        private readonly bool _isInverted;

        private NamedTypeMetaInfo Type => _member.ContainingFullType;

        public string FormatterName => "IgnoreIf";

        public IgnoreIfResolver(MemberMetaInfo member, ConditionExpression condition, IConditionAnalyzer analyzer, bool isInverted) {
            _member = member;
            _expression = condition;
            _condition = analyzer;
            _isInverted = isInverted;
        }

        private string GetConditionString(IChainMethodArgsProvider provider) => _condition.GetEvalString(provider[typeof(TypeSourceArgs)], Type, _expression);

        private string MethodBase(TypeSourceArgs source, string nextMethodInvoke) {
            var node = new CodeNodeBuilder();

            node.If($"{(_isInverted ? "!" : "")}({_condition.GetEvalString(source.ToString(), Type, _expression)})", node => {
                node.Return(nextMethodInvoke);
            });
            node.Return(DeserializeResult.Success.ToDisplayString());

            return node.ToString();
        }

        public string GetMethodBody(string methodId, IChainMethodArgsProvider provider, IComponentProvider components, IThrowCollection throwCollection) {
            var node = new CodeNodeBuilder();

            node.If($"{IsInvert()}({GetConditionString(provider)})", node => {
                node.Return(provider.InvokeNextMethod());
            });
            node.Return(DeserializeResult.Success.ToDisplayString());

            return node.ToString();

            string IsInvert() => _isInverted ? "!" : "";
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
