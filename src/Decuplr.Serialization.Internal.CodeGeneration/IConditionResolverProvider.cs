using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IConditionResolverProvider : IValidationSource {
        IConditionalFormatter GetResolver(MemberMetaInfo member, IThrowCollection throwCollection);
    }
}
