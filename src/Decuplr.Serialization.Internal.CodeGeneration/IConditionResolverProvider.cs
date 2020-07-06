using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IConditionResolverProvider : IConditionValidatable {
        IConditionResolver GetResolver(MemberMetaInfo member);
    }
}
