using Decuplr.Serialization.Binary.AnalysisService;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal interface IOrderSelector {
        BinaryLayout GetAutoLayoutImplication(TypeMetaInfo type);

        bool ShouldSelect(MemberMetaInfo memberInfo, BinaryLayout layout, IDiagnosticReporter diagnostic);

        int GetRelativeOrder(MemberMetaInfo memberInfo);
    }
}
