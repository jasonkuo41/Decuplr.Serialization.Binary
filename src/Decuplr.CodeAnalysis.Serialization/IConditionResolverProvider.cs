using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Diagnostics;
using System.Collections.Generic;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IConditionResolverProvider : IGroupValidationProvider {
        bool ShouldFormat(MemberMetaInfo member);
        IEnumerable<IConditionResolver> GetResolvers(MemberMetaInfo member, IThrowCollection throwCollection);
    }
}
