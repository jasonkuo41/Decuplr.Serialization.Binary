using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal class MemberComposerFactory {

        private readonly IEnumerable<IConditionResolverProvider> _conditions;
        private readonly IEnumerable<IMemberDataFormatterProvider> _formatters;
        private readonly ISourceAddition _sourceAddition;
        private readonly ITypeSymbolProvider _symbolProvider;

        public MemberComposerFactory(IEnumerable<IConditionResolverProvider> conditions,
                                     IEnumerable<IMemberDataFormatterProvider> formatters,
                                     ISourceAddition sourceAddition,
                                     ITypeSymbolProvider symbolProvider) {
            _conditions = conditions;
            _formatters = formatters;
            _sourceAddition = sourceAddition;
            _symbolProvider = symbolProvider;
        }

        public IMemberComposer Build(ITypeComposer typeComposer, MemberMetaInfo meta, GeneratingTypeName typeName, IComponentProvider provider)
            => new MemberComposerSource(meta, typeName, _conditions, _formatters, _symbolProvider)
                .CreateStruct(typeComposer, provider, (name, source) => {
                    _sourceAddition.AddSource($"{typeName}.generated.cs", source);
                    return _symbolProvider.GetSymbol(name.ToString());
                });
    }
}
