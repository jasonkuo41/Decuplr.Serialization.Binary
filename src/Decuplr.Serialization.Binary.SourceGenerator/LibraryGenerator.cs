using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Decuplr.Serialization.Binary.FormatSource;
using Decuplr.Serialization.Binary.ParserProviders;
using Decuplr.Serialization.CodeGeneration;
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

        public void Initialize(InitializationContext context) => context.RegisterForSyntaxNotifications(() => new CandidateSyntaxReceiver());

        public void Execute(SourceGeneratorContext context) {
            try {
                if (!(context.SyntaxReceiver is CandidateSyntaxReceiver receiver))
                    return;

                var builder = new CodeGeneratorBuilder();

                builder.AddStartup<BinaryFormatProvider>();
                builder.AddStartup<BinaryParserProvider>();

                var generator = builder.UseDependencyProvider<InlineDependencyProvider>()
                                       .UseDependencyProvider<DefaultDependencyProvider>()
                                       .CreateGenerator();

                generator.GenerateFiles(receiver.DeclaredTypes, context.Compilation, context.CancellationToken);
            }
            catch (Exception e) {
                context.WriteException(e);
            }
        }

    }

}
