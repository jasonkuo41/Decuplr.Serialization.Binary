using System.Collections.Generic;
using Decuplr.Serialization.Binary.AnalysisService;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService.Internal {
    internal interface IInvalidDiagnosticConditions : IConditionRules {
        IEnumerable<Diagnostic> GetDiagnsotics(IEnumerable<MemberMetaInfo> member);
    }

}
