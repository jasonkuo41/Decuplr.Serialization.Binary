using System;
using System.Collections.Generic;
using Decuplr.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class DiagnosticReporter : IDiagnosticReporter {

        private readonly Action<Diagnostic>? _diagnostics;

        public bool ContainsError { get; private set; }

        public DiagnosticReporter(Action<Diagnostic>? diagnosticAction) {
            _diagnostics = diagnosticAction;
        }

        public void ReportDiagnostic(Diagnostic diagnostic) {
            if (diagnostic.Severity == DiagnosticSeverity.Error)
                ContainsError = true;
            _diagnostics?.Invoke(diagnostic);
        }

        public void ReportDiagnostic(IEnumerable<Diagnostic> diagnostics) {
            foreach(var diagnostic in diagnostics)
                ReportDiagnostic(diagnostic);
        }
    }
}
