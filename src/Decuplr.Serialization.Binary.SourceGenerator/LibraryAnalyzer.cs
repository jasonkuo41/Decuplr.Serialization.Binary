using System.Collections.Immutable;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Decuplr.Serialization.Binary {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LibraryAnalyzer : DiagnosticAnalyzer {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticHelperLegacy.CannotFindMatchingParserType);


        public override void Initialize(AnalysisContext context) {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(SyntaxAnalysis, SyntaxKind.StructDeclaration, SyntaxKind.ClassDeclaration);
        }

        public void SyntaxAnalysis(SyntaxNodeAnalysisContext context) {
            if (!(context.Node is TypeDeclarationSyntax typeDeclareSyntax))
                return;
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelperLegacy.CannotFindMatchingParserType, typeDeclareSyntax.GetLocation()));
            foreach (var type in SourceCodeAnalyzer.AnalyzeTypeSyntax(new TypeDeclarationSyntax[] { typeDeclareSyntax }, context.Compilation, context.CancellationToken)) {
                var precusor = new SchemaPrecusor {
                    NeverDeserialize = false,
                    IsSealed = true,
                    RequestLayout = LayoutOrder.Auto,
                    TargetNamespaces = new string[] { "Default" }
                };
                TypeFormatLayout.TryGetLayout(type, ref precusor, out var diagnostics, out _);
                foreach (var diagnostic in diagnostics)
                    context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
