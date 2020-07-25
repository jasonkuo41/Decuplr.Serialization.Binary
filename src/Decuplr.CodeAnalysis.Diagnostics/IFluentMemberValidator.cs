using System;
using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface IFluentMemberValidator {
        ISymbolRule<MemberMetaInfo> WhereMember(Func<MemberMetaInfo, bool> predicate);
        IAttributeRule<MemberMetaInfo> WhereAttribute<TAttribute>() where TAttribute : Attribute;
    }
}
