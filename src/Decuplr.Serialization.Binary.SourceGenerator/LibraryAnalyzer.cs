using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Decuplr.Serialization.Binary {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LibraryAnalyzer : DiagnosticAnalyzer {

        private readonly ICodeGeneratorFactory _generatorFactory = CommonGeneratorSetup.CreateCommonFactory();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.CreateRange(DiagnosticHelper.Descriptors.Select(x => x.Value));

        public override void Initialize(AnalysisContext context) {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSymbolAction(SymbolAnalysis, SymbolKind.NamedType);
        }

        public void SymbolAnalysis(SymbolAnalysisContext context) {
            if (!(context.Symbol is INamedTypeSymbol symbol))
                return;
            var syntaxes = symbol.DeclaringSyntaxReferences.Select(x => (TypeDeclarationSyntax)x.GetSyntax(context.CancellationToken));
            _generatorFactory.RunGeneration(context.Compilation, syntaxes, generator => {
                generator.OnReportedDiagnostic += (_, diagnostic) => context.ReportDiagnostic(diagnostic);
                generator.VerifySyntax();
            });
        }
    }
}
