using System.Collections.Generic;
using Decuplr.Serialization.Binary.AnalysisService;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal interface IOrderSelector {
        void ElectMember(ILayoutMemberCollection filter);
        IEnumerable<MemberMetaInfo> GetOrder(IEnumerable<MemberMetaInfo> memberInfo, BinaryLayout layout, IDiagnosticReporter diagnostic);
    }
}
