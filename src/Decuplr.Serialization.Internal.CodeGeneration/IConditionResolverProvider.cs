using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.TypeComposers;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IConditionResolverProvider : IValidationSource {
        bool ShouldFormat(MemberMetaInfo member);
        IConditionalFormatter GetResolver(MemberMetaInfo member, IThrowCollection throwCollection);
    }
}
