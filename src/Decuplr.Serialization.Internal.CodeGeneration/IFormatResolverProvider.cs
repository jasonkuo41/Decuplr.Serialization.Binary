using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IFormatResolverProvider {
        void ValidConditions(ITypeValidator validator);
        IFormatResolver GetResolver(MemberMetaInfo member, IDependencyProvider provider);
    }
}
