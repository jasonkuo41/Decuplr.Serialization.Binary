using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal class MemberComposerFactory {

        private readonly IEnumerable<IConditionResolverProvider> _conditions;
        private readonly IEnumerable<IMemberDataFormatterProvider> _formatters;

        public MemberComposerFactory(IEnumerable<IConditionResolverProvider> conditions, IEnumerable<IMemberDataFormatterProvider> formatters) {
            _conditions = conditions;
            _formatters = formatters;
        }

        public IMemberComposer Build(MemberMetaInfo meta, GeneratingTypeName typeName, IComponentProvider provider)
            => new MemberComposerSource(meta, typeName, _conditions, _formatters).CreateStruct(provider);
    }
}
