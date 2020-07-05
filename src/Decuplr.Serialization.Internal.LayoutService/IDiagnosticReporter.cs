using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.LayoutService {
    public interface IDiagnosticReporter {
        void ReportDiagnostic(Diagnostic diagnostic);
        void ReportDiagnostic(IEnumerable<Diagnostic> diagnostics);
    }
}
