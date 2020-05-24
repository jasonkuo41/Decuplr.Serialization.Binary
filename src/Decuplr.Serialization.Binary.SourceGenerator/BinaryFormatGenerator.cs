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

        class CandidateSyntaxReceiver : ISyntaxReceiver {

            public List<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
                // We capture every class we are interested in
                if (syntaxNode is TypeDeclarationSyntax classSyntax && classSyntax.AttributeLists.Any())
                    CandidateTypes.Add(classSyntax);
                // TODO : Add interfaces, note we may only allow interfaces with public setters
            }
        }

        public void Initialize(InitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new CandidateSyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context) {
            if (!(context.SyntaxReceiver is CandidateSyntaxReceiver receiver))
                return;
            var compilation = context.Compilation;
            var attributeSymbol = compilation.GetTypeByMetadataName(typeof(BinaryFormatAttribute).FullName);
            var indexSymbol = compilation.GetTypeByMetadataName(typeof(IndexAttribute).FullName);

            var sourceBuilder = new StringBuilder(@"using System;
                namespace Decuplr.Serialization.Binary {
                    public static class DebugContent {
                        public static void PrintDebugInfo() {
                            Console.WriteLine(""Start of debug info"");
                ");

            try {
                var qualifiedSymbols = new List<TypeAnalyzer>();
                foreach (var candidateClass in receiver.CandidateTypes) {
                    var model = compilation.GetSemanticModel(candidateClass.SyntaxTree);
                    var typeSymbol = model.GetDeclaredSymbol(candidateClass);
                    if (typeSymbol.GetAttributes().Any(x => x.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default))) {
                        var shouldPartial = typeSymbol.GetMembers()
                            .Where(member => member is IFieldSymbol || member is IPropertySymbol)
                            .Where(member => !member.IsImplicitlyDeclared)
                            .Where(member => member.GetAttributes().Any(x => x.AttributeClass.Equals(indexSymbol, SymbolEqualityComparer.Default)))
                            .All(member => member.CanAccessSymbolInternally());

                        sourceBuilder.AppendLine($"Console.WriteLine(\"{typeSymbol}\");");
                        sourceBuilder.AppendLine($"Console.WriteLine(\"   {string.Join(" , ", candidateClass.Modifiers.Select(x => x.ValueText))}\");");
                        /*
                        if (!candidateClass.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) {
                            var report = new DiagnosticDescriptor("CD1001", "Type should not be partial", "Formatting", "Decuplr.Bi", DiagnosticSeverity.Error, true);
                            context.ReportDiagnostic(Diagnostic.Create(report, candidateClass.Modifiers.Where(x => x.IsKind(SyntaxKind.PartialKeyword)).First().GetLocation()));
                        }
                        */
                        var analyzedSymbol = new TypeAnalyzer(candidateClass, typeSymbol);
                        //analyzedSymbol.AddToContextSource(context);
                        qualifiedSymbols.Add(analyzedSymbol);

                        sourceBuilder.AppendLine($"Console.WriteLine(@\"{PartialClassBuilder.CreatePartialClass(typeSymbol, new TypeLayout[] { })}\");");
                    }
                }
            } catch (Exception e) {
                sourceBuilder.AppendLine($"Console.WriteLine(@\"{e} {e.Message} \r \n {e.StackTrace}\");");
            }
            // After all the generated code, we will need to define an entry point
            // They are pretty much global and wouldn't use namespace
            //context.AddSource("SerializerEntryPoint.cs", SourceText.From(EntryPointGenerator.CreateSourceText(qualifiedSymbols)));

            sourceBuilder.Append(@"} } }");
            context.AddSource("DebugInfo.BinaryFormatter.Generated.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

    }

}
