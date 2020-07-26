using Decuplr.Serialization.AnalysisService;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IConditionResolverProvider : IValidationSource {
        bool ShouldFormat(MemberMetaInfo member);
        IConditionalFormatter GetResolver(MemberMetaInfo member, IThrowCollection throwCollection);
    }
}
