using System;
using System.Collections.Generic;
using Decuplr.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class DiagnosticReporter : IDiagnosticReporter {

        public event EventHandler<Diagnostic>? OnReportedDiagnostic;

        public bool ContainsError { get; private set; }

        public void ReportDiagnostic(Diagnostic diagnostic) {
            if (diagnostic.Severity == DiagnosticSeverity.Error)
                ContainsError = true;
            OnReportedDiagnostic?.Invoke(this, diagnostic);
        }

        public void ReportDiagnostic(IEnumerable<Diagnostic> diagnostics) {
            foreach(var diagnostic in diagnostics)
                ReportDiagnostic(diagnostic);
        }
    }
}
