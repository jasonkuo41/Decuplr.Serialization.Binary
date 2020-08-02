using System.Collections.Generic;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IOrderSelector : ITypeValidationProvider {
        /// <summary>
        /// Should the diagnostic continue even an error occurs
        /// </summary>
        bool ContinueDiagnosticAfterError { get; }

        /// <summary>
        /// Generates and reorders the member to the defined order, with error reporting capability
        /// </summary>
        /// <param name="typeInfo">The elected member that shall be ordered</param>
        /// <returns>The order of the members</returns>
        IReadOnlyList<MemberMetaInfo> GetOrder(NamedTypeMetaInfo typeInfo);
    }
}
