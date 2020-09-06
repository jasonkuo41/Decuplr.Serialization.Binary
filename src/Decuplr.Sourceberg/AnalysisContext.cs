using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
    /// <summary>
    /// Provides context to the current analysis
    /// </summary>
    /// <typeparam name="TCurrent">The symbol or syntax the current analysis is performing on.</typeparam>
    public struct AnalysisContext<TCurrent> {
        private readonly Func<Diagnostic, bool> _supportedDiagnostic;
        private readonly Action<Diagnostic> _reportDiagnostic;

        public TCurrent Current { get; }

        public ISourceFeatureCollection Features { get; }

        public CancellationToken CancellationToken { get; }

        public AnalysisContext(TCurrent current, ISourceFeatureCollection features, CancellationToken ct, Action<Diagnostic> reportDiagnostic, Func<Diagnostic, bool> isSupportedDiagnostic) {
            Current = current;
            Features = features;
            CancellationToken = ct;
            _reportDiagnostic = reportDiagnostic;
            _supportedDiagnostic = isSupportedDiagnostic;
        }

        public void ReportDiagnostic(Diagnostic diagnostic) {
            if (_supportedDiagnostic?.Invoke(diagnostic) ?? true)
                throw new ArgumentException($"Reported diagnostic with ID '{diagnostic.Id}' is not supported by the analyzer.", nameof(diagnostic));
            _reportDiagnostic?.Invoke(diagnostic);
        }

        public void ReportDiagnostic(IEnumerable<Diagnostic> diagnostics) {
            foreach (var diagnostic in diagnostics) {
                ReportDiagnostic(diagnostic);
            }
        }
    }
}
