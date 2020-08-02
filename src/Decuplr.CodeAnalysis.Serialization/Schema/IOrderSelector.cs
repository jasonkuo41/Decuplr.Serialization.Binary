using System.Collections.Generic;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IOrderSelector : ITypeValidationProvider {
        
        /// <summary>
        /// If a member should consider to be verified
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        bool IsCandidateMember(MemberMetaInfo member);

        /// <summary>
        /// Generates and reorders the member to the defined order, with error reporting capability
        /// </summary>
        /// <param name="typeInfo">The elected member that shall be ordered</param>
        /// <returns>The order of the members</returns>
        IReadOnlyList<MemberMetaInfo> GetOrder(NamedTypeMetaInfo typeInfo);
    }
}
