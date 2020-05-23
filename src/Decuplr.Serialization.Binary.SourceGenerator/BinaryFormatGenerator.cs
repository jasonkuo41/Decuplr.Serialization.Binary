using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    [Generator]
    public class BinaryFormatGenerator : ISourceGenerator {
        public void Initialize(InitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context) {
            if (!(context.SyntaxReceiver is MySyntaxReceiver receiver))
                return;
            var compilation = context.Compilation;
            var attributeSymbol = compilation.GetTypeByMetadataName(typeof(BinaryFormatAttribute).FullName);

            var sourceBuilder = new StringBuilder(@"
using System;
namespace TestGenerated {
    public static class Analyzed {
        public static void PrintResult() {
            Console.WriteLine(""Entry"");
                ");

            List<ITypeSymbol> matchedTypeSymbols = new List<ITypeSymbol>();
            foreach(var candidateClass in receiver.CandidateTypes) {
                var model = compilation.GetSemanticModel(candidateClass.SyntaxTree);
                var typeSymbol = model.GetDeclaredSymbol(candidateClass) as ITypeSymbol;
                if (typeSymbol.GetAttributes().Any(x => x.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default))) {
                    matchedTypeSymbols.Add(typeSymbol);
                    sourceBuilder.Append($"Console.WriteLine(\"{typeSymbol.Name}\");");
                    sourceBuilder.Append($"Console.WriteLine(\"{typeSymbol.ToString()}\");");
                }
            }

            // Don't need to proceed if we have nothing
            if (matchedTypeSymbols.Count == 0)
                return;



            sourceBuilder.Append(@"} } }");
            context.AddSource(nameof(BinaryFormatGenerator), SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

    }

    class MySyntaxReceiver : ISyntaxReceiver {

        public List<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            // We capture every class we are interested in
            if (syntaxNode is TypeDeclarationSyntax classSyntax && classSyntax.AttributeLists.Any())
                CandidateTypes.Add(classSyntax);
            // TODO : Add interfaces, note we may only allow interfaces with public setters
        }
    }

}
