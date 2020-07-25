using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics.Internal {
    internal interface IInvalidDiagnosticConditions : IConditionRules {
        IEnumerable<Diagnostic> GetDiagnsotics(IEnumerable<MemberMetaInfo> member);
    }

}
