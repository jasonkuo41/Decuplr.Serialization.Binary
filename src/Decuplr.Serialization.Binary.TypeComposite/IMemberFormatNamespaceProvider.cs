using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public interface IMemberFormatNamespaceProvider {
        FormatNamespaceInfo GetUsingNamespaces(MemberMetaInfo member);
    }
}
