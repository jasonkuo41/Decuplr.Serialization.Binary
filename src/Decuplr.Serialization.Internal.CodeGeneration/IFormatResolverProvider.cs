using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IFormatResolverProvider : IValidationSource {
        bool TryGetResolver(MemberMetaInfo member, IComponentCollection provider, out IFormatResolver resolver);
    }
}
