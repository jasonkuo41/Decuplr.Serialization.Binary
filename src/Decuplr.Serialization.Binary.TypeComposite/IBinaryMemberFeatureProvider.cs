using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public interface IBinaryMemberFeatureProvider {
        bool ShouldApply(MemberMetaInfo member);
        IBinaryMemberComposeMethod GetComposingMethods(MemberMetaInfo member, IComponentCollection components, IThrowCollection throwCollection);
    }
}
