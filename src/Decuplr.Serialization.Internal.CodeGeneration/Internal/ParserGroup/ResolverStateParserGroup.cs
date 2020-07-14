using System;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.Serialization.CodeGeneration.Internal.ParserGroup {
    internal class ResolverStateParserGroup : StateParserGroup {

        private readonly IFormmaterBase<TypeSourceArgs> _resolver;

        public ResolverStateParserGroup(IFormmaterBase<TypeSourceArgs> resolver, MemberMetaInfo member, int index) : base(member, index) {
            _resolver = resolver;
        }

        protected override void DeserializeSequence(CodeNodeBuilder node, DeserializeSequenceArgs<TypeSourceArgs> args) => node.Add(_resolver.GetFunctionBody(Method.DeserializeState(Index + 1), args));

        protected override void DeserializeSpan(CodeNodeBuilder node, DeserializeSpanArgs<TypeSourceArgs> args) => node.Add(_resolver.GetFunctionBody(Method.DeserializeState(Index + 1), args));

        protected override void GetLength(CodeNodeBuilder node, GetLengthArgs<TypeSourceArgs> args) => node.Add(_resolver.GetFunctionBody(Method.GetLengthState(Index + 1), args));

        protected override void Serialize(CodeNodeBuilder node, SerializeArgs<TypeSourceArgs> args) => node.Add(_resolver.GetFunctionBody(Method.SerializeState(Index + 1), args));

        protected override void TryDeserializeSequence(CodeNodeBuilder node, TryDeserializeSequenceArgs<TypeSourceArgs> args) => node.Add(_resolver.GetFunctionBody(Method.TryDeserializeState(Index + 1), args));

        protected override void TryDeserializeSpan(CodeNodeBuilder node, TryDeserializeSpanArgs<TypeSourceArgs> args) => node.Add(_resolver.GetFunctionBody(Method.TryDeserializeState(Index + 1), args));

        protected override void TrySerialize(CodeNodeBuilder node, TrySerializeArgs<TypeSourceArgs> args) => node.Add(_resolver.GetFunctionBody(Method.TrySerializeState(Index + 1), args));
    }
}
