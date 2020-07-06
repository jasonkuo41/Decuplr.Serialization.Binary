using Decuplr.Serialization.AnalysisService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IFormatResolverProvider : IConditionValidatable {
        IFormatResolver GetResolver(MemberMetaInfo member, IDependencyProvider provider);
    }
}
