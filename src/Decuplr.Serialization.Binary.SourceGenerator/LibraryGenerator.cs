using System;
using System.Collections.Generic;
using Decuplr.CodeAnalysis.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary {

    [Generator]
    public class LibraryGenerator : ISourceGenerator {

        private class CandidateSyntaxReceiver : ISyntaxReceiver {

            public List<TypeDeclarationSyntax> DeclaredTypes { get; } = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
                // We capture every class we are interested in
                // We only capture classes that comes with attribute, but we may also be interested in those with specific syntax ending
                if (syntaxNode is TypeDeclarationSyntax classSyntax && classSyntax.AttributeLists.Any()) {
                    DeclaredTypes.Add(classSyntax);
                }
                // TODO : Add interfaces, note we may only allow interfaces with public setters
            }
        }

        private readonly ICodeGeneratorFactory _genFactory = CommonGeneratorSetup.CreateCommonFactory();

        public void Initialize(InitializationContext context) => context.RegisterForSyntaxNotifications(() => new CandidateSyntaxReceiver());

        public void Execute(SourceGeneratorContext context) {
            try {
                if (!(context.SyntaxReceiver is CandidateSyntaxReceiver receiver))
                    return;

                _genFactory.RunGeneration(context.Compilation, receiver.DeclaredTypes, generator => {
                    generator.OnReportedDiagnostic += (_, diagnostic) => context.ReportDiagnostic(diagnostic);
                    generator.OnGeneratedSource += (_, source) => context.AddSourceWithDebug(source);
                    generator.GenerateFiles();
                });
            }
            catch (Exception e) {
                context.WriteException(e);
            }
        }

    }
}
