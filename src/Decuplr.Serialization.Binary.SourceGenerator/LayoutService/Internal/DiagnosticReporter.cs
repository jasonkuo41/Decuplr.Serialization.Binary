using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService.Internal {
    class DiagnosticReporter : IDiagnosticReporter {

        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public bool IsUnrecoverable { get; private set; }

        public IEnumerable<Diagnostic> ExportDiagnostics() => _diagnostics;

        public void ReportDiagnostic(Diagnostic diagnostic) {
            if (diagnostic.Severity == DiagnosticSeverity.Error)
                IsUnrecoverable = true;
            _diagnostics.Add(diagnostic);
        }

        public void ReportDiagnostic(IEnumerable<Diagnostic> diagnostics) {
            foreach (var diagnostic in diagnostics)
                ReportDiagnostic(diagnostic);
        }
    }
}
