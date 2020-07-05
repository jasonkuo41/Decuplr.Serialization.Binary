using System.Collections.Generic;
using Decuplr.Serialization.AnalysisService;

namespace Decuplr.Serialization.LayoutService {
    public interface IOrderSelector {
        void ValidateMembers(ILayoutMemberValidation filter);
        IEnumerable<MemberMetaInfo> GetOrder(IEnumerable<MemberMetaInfo> memberInfo, LayoutOrder layout, IDiagnosticReporter diagnostic);
    }
}
