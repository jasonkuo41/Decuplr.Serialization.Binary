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

        private struct TypeSymbolInfo {
            public ITypeSymbol Type { get; set; }
            public bool ShouldPartial { get; set; }
        }

        public void Initialize(InitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context) {
            if (!(context.SyntaxReceiver is MySyntaxReceiver receiver))
                return;
            var compilation = context.Compilation;
            var attributeSymbol = compilation.GetTypeByMetadataName(typeof(BinaryFormatAttribute).FullName);
            var indexSymbol = compilation.GetTypeByMetadataName(typeof(IndexAttribute).FullName);

            var sourceBuilder = new StringBuilder(@"
using System;
namespace TestGenerated {
    public static class Analyzed {
        public static void PrintResult() {
            Console.WriteLine(""Entry"");
                ");

            var matchedTypeSymbols = new List<TypeSymbolInfo>();
            foreach(var candidateClass in receiver.CandidateTypes) {
                var model = compilation.GetSemanticModel(candidateClass.SyntaxTree);
                var typeSymbol = model.GetDeclaredSymbol(candidateClass) as ITypeSymbol;
                if (typeSymbol.GetAttributes().Any(x => x.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default))) {
                    var shouldPartial = typeSymbol.GetMembers()
                        .Where(member => member is IFieldSymbol || member is IPropertySymbol)
                        .Where(member => !member.IsImplicitlyDeclared)
                        .Where(member => member.GetAttributes().Any(x => x.AttributeClass.Equals(indexSymbol, SymbolEqualityComparer.Default)))
                        .All(member => CanAccessSymbol(member));

                    matchedTypeSymbols.Add(new TypeSymbolInfo {
                        Type = typeSymbol,
                        ShouldPartial = shouldPartial
                    });

                    sourceBuilder.Append($"Console.WriteLine(\"{typeSymbol}\");");
                    foreach (var member in typeSymbol.GetMembers()) {
                        sourceBuilder.Append($"Console.WriteLine(\"\\t {member.GetType()} : {member.Name}, Length : {(member as IFieldSymbol)?.CustomModifiers.Length}\");");
                    }
                }
            }

            // Don't need to proceed if we have nothing
            if (matchedTypeSymbols.Count == 0)
                return;



            sourceBuilder.Append(@"} } }");
            context.AddSource(nameof(BinaryFormatGenerator), SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        private bool CanAccessSymbol(ISymbol member) {
            if (member is IFieldSymbol fieldSymbol)
                return CanFormatterAccess(member.DeclaredAccessibility);
            if (member is IPropertySymbol propSymbol)
                return !propSymbol.IsReadOnly && CanFormatterAccess(propSymbol.SetMethod.DeclaredAccessibility))
            return false;
        }

        private static bool CanFormatterAccess(Accessibility accessibility) => accessibility switch
        {
            Accessibility.Public => true,
            Accessibility.Internal => true,
            Accessibility.ProtectedOrInternal => true,
            Accessibility.ProtectedAndInternal => true,
            _ => false
        };
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
