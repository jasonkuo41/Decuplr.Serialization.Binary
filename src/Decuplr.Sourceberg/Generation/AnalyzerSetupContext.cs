namespace Decuplr.Sourceberg.Generation {
    public class AnalyzerSetupContext {
        public ISyntaxAnalyzerSetup SyntaxAnalyzerSetup { get; }
        public ISymbolAnalyzerSetup SymbolAnalyzerSetup { get; }

        internal AnalyzerSetupContext(ISyntaxAnalyzerSetup syntaxSetup, ISymbolAnalyzerSetup symbolSetup) {
            SyntaxAnalyzerSetup = syntaxSetup;
            SymbolAnalyzerSetup = symbolSetup;
        }
    }
}
