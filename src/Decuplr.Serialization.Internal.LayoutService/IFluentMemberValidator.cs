using System;
using Decuplr.Serialization.AnalysisService;

namespace Decuplr.Serialization.LayoutService {
    public interface IFluentMemberValidator {
        ISymbolRule<MemberMetaInfo> WhereMember(Func<MemberMetaInfo, bool> predicate);
        IAttributeRule<MemberMetaInfo> WhereAttribute<TAttribute>() where TAttribute : Attribute;
    }
}
