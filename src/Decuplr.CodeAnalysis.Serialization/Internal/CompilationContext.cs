using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class CompilationContext {
        public SourceCodeAnalysis? SymbolProvider { get; set; }
        public ISourceAddition? SourceProvider { get; set; }
        public IDiagnosticReporter? DiagnosticReporter { get; set; }
        public ICompilationInfo? CompilationInfo { get; set; }

        protected void CopyFrom(CompilationContext context) {
            SymbolProvider = context.SymbolProvider;
            SourceProvider = context.SourceProvider;
            DiagnosticReporter = context.DiagnosticReporter;
        }
    }

}
