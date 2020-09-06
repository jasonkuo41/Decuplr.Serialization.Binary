using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Sourceberg.Generation {
    public abstract class GeneratorStartup {
        public abstract bool ShouldCaptureSyntax(SyntaxNode syntaxNode, CancellationToken ct);
        public abstract void ConfigureAnalyzer(AnalyzerSetupContext setup);
        public abstract void ConfigureServices(IServiceCollection services);
    }

    public class AnalyzerSetupContext {
        public ISyntaxAnalyzerSetup SyntaxAnalyzerSetup { get; }
        public ISymbolAnalyzerSetup SymbolAnalyzerSetup { get; }

        internal AnalyzerSetupContext(ISyntaxAnalyzerSetup syntaxSetup, ISymbolAnalyzerSetup symbolSetup) {
            SyntaxAnalyzerSetup = syntaxSetup;
            SymbolAnalyzerSetup = symbolSetup;
        }
    }
}
