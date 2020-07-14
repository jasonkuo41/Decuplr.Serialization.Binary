using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IMemberDataFormatterProvider : IValidationSource {
        bool TryGetResolver(MemberMetaInfo member, IComponentCollection provider, IThrowCollection throwCollection, out IMemberDataFormatter resolver);
    }
}
