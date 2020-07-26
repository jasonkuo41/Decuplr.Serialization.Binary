using System.Collections.Generic;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IOrderSelector : ITypeValidationProvider {
        /// <summary>
        /// Should the validation proccess continue to select the orders even if the member validation fails
        /// </summary>
        /// <remarks>
        /// By setting this option to true, allows diagnostics to be generated if any issue was found with ordering even if there's faulty members
        /// </remarks>
        bool ContinueOnFailedValidation { get; }

        bool IsValidMember(MemberMetaInfo member);

        /// <summary>
        /// Generates and reorders the member to the defined order, with error reporting capability
        /// </summary>
        /// <param name="memberInfo">The elected member that shall be ordered</param>
        /// <param name="diagnostic">A reporter to report diagnostics to</param>
        /// <returns>The order of the members</returns>
        IEnumerable<MemberMetaInfo> GetOrder(IEnumerable<MemberMetaInfo> memberInfo);
    }
}
