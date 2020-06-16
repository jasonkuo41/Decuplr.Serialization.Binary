using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Decuplr.Serialization.Binary {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class LibraryAnalyzer : DiagnosticAnalyzer {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => throw new NotImplementedException();

        public override void Initialize(AnalysisContext context) {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(SyntaxAnalysis, SyntaxKind.StructDeclaration | SyntaxKind.ClassDeclaration);
        }

        public void SyntaxAnalysis(SyntaxNodeAnalysisContext context) {
            if (!(context.Node is TypeDeclarationSyntax typeDeclareSyntax))
                return;

        }
    }
}
