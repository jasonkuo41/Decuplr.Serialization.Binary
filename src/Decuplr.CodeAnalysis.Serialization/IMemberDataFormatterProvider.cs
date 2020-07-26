using Decuplr.Serialization.AnalysisService;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IMemberDataFormatterProvider : IValidationSource {
        bool ShouldFormat(MemberMetaInfo member);
        IMemberDataFormatter GetFormatter(MemberMetaInfo member, IComponentCollection provider, IThrowCollection throwCollection);
    }
}
