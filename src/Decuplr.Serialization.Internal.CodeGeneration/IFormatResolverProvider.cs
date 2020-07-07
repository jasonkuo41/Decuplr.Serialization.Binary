using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IFormatResolverProvider : IValidationSource {
        IFormatResolver GetResolver(MemberMetaInfo member, IDependencyProvider provider);
    }
}
