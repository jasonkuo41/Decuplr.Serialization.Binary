using System;
using System.Collections.Generic;
using System.Linq;

namespace Decuplr.Sourceberg {
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class UseDiagnosticsAttribute : Attribute {
        public UseDiagnosticsAttribute(string diagnostics, params string[] diagnosticIds) {
            SupportedDiagnostics = diagnosticIds.Prepend(diagnostics);
        }

        public IEnumerable<string> SupportedDiagnostics { get; }
    }
}
