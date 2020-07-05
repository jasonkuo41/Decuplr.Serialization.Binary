using System.Collections.Generic;
using Decuplr.Serialization.AnalysisService;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.LayoutService.Internal {
    internal interface IInvalidDiagnosticConditions : IConditionRules {
        IEnumerable<Diagnostic> GetDiagnsotics(IEnumerable<MemberMetaInfo> member);
    }

}
