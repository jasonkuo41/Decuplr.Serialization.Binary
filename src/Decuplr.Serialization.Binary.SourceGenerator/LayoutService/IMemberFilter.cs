using System.Collections.Generic;
using Decuplr.Serialization.Binary.AnalysisService;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal interface IMemberFilter {
        IEnumerable<MemberMetaInfo> SelectMembers(IEnumerable<MemberMetaInfo> source);
    }
}
