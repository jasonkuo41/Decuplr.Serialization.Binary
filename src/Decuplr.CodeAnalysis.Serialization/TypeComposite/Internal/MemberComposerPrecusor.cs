using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    /// <summary>
    /// A prototype of the dependency struct
    /// </summary>
    internal class MemberComposerPrecusor {

        private readonly CodeNodeBuilder _nodeBuilder;

        public MemberMetaInfo Target { get; }

        public string Name { get; }

        public string SourceCode => _nodeBuilder.ToString();

        public MemberComposerPrecusor(MemberMetaInfo metaInfo, string name, CodeNodeBuilder nodeBuilder) {
            Target = metaInfo;
            Name = name;
            _nodeBuilder = nodeBuilder;
        }

        public void EmbedSourceCode(CodeNodeBuilder builder) => builder.AddPlain(SourceCode);
    }
}
