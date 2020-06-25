using System;
using Decuplr.Serialization.Binary.AnalysisService;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal interface ILayoutMemberCollection {
        ISymbolRule<MemberMetaInfo> WhereMember(Func<MemberMetaInfo, bool> predicate);
        IAttributeRule<MemberMetaInfo> WhereAttribute<TAttribute>() where TAttribute : Attribute;
    }
}
