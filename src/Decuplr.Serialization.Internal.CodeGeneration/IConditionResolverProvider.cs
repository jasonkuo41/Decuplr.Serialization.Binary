using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IConditionResolverProvider : IValidationSource {
        IConditionResolver GetResolver(MemberMetaInfo member);
    }
}
