using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal interface IDiagnosticReporter {
        void ReportDiagnostic(Diagnostic diagnostic);
    }
}
