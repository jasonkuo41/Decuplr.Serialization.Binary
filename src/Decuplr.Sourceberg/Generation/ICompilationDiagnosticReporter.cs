using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Generation {
    public interface ICompilationDiagnosticReporter {
        bool ContainsError { get; }
        void ReportDiagnostic(Diagnostic diagnostic);
        void ReportDiagnostic(IEnumerable<Diagnostic> diagnostics);
    }
}
