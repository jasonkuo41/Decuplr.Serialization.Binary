using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IMemberDataFormatterProvider : IGroupValidationProvider {
        bool ShouldFormat(MemberMetaInfo member);
        IMemberDataFormatter GetFormatter(MemberMetaInfo member, IComponentCollection provider, IThrowCollection throwCollection);
    }
}
