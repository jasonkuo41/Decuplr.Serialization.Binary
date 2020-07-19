using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.TypeComposers;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IMemberDataFormatterProvider : IValidationSource {
        bool ShouldFormat(MemberMetaInfo member);
        IMemberDataFormatter GetFormatter(MemberMetaInfo member, IComponentCollection provider, IThrowCollection throwCollection);
    }
}
