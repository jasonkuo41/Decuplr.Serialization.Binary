using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IConditionResolverProvider {
        void ValidConditions(ITypeValidator validator);
        IConditionResolver GetResolver(MemberMetaInfo member);
    }
}
