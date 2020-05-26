using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Decuplr.Serialization.Binary.SourceGenerator.Solutions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
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
            var binaryFormatSymbol = compilation.GetTypeByMetadataName(typeof(BinaryFormatAttribute).FullName);
            var indexSymbol = compilation.GetTypeByMetadataName(typeof(IndexAttribute).FullName);

            Debug.Assert(binaryFormatSymbol != null);
            Debug.Assert(indexSymbol != null);

            var sourceBuilder = new StringBuilder(@"using System;
                namespace Decuplr.Serialization.Binary {
                    public static class DebugContent {
                        public static void PrintDebugInfo() {
                            Console.WriteLine(""Start of debug info"");
                ");

            var candidateSymbols = new HashSet<INamedTypeSymbol>();
            foreach (var candidateClass in receiver.CandidateTypes) {
                var model = compilation.GetSemanticModel(candidateClass.SyntaxTree, true);
                var typeSymbol = model.GetDeclaredSymbol(candidateClass);
                if (typeSymbol is null || !typeSymbol.GetAttributes().HasAny(binaryFormatSymbol!))
                    continue;
                sourceBuilder.AppendLine($"Console.WriteLine(\"{typeSymbol}\");");
                sourceBuilder.AppendLine($"Console.WriteLine(\"   {string.Join(" , ", candidateClass.Modifiers.Select(x => x.ValueText))}\");");
                candidateSymbols.Add(typeSymbol!);
            }

            sourceBuilder.AppendLine($"Console.WriteLine(\"\");");
            try {
                // For debug generator
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "Generated");
                Directory.CreateDirectory(dir);

                var result = new List<TypeFormatterBuilder>();
                foreach (var typeSymbol in candidateSymbols) {
                    sourceBuilder.AppendLine($"Console.WriteLine(\"{typeSymbol}\");");
                    if (!TypeInfoDiscovery.TryParseType(typeSymbol, context, out var typeInfo))
                        continue;

                    var serializer = new PartialTypeSerialize(typeInfo!);
                    var deserializer = new PartialTypeDeserialize(typeInfo!);
                    result.Add(new TypeFormatterBuilder(typeInfo!, deserializer, serializer));
                }

                // Generate an entry point
                context.AddSource(EntryPointBuilder.CreateSourceText(result), Encoding.UTF8);

                // Generate those additional files
                foreach (var additionFile in result.SelectMany(x => x.AdditionalCode))
                    context.AddSource(additionFile, Encoding.UTF8);
            }
            catch (Exception e) {
                sourceBuilder.AppendLine($"Console.WriteLine(@\"{e} {e.Message} \r \n {e.StackTrace}\");");
            }
            // After all the generated code, we will need to define an entry point
            // They are pretty much global and wouldn't use namespace
            //context.AddSource("SerializerEntryPoint.cs", SourceText.From(EntryPointGenerator.CreateSourceText(qualifiedSymbols)));

            sourceBuilder.Append(@"} } }");
            context.AddSource("DebugInfo.BinaryFormatter.Generated.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));

        }

    }

    /*
    if (!candidateClass.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) {
        var report = new DiagnosticDescriptor("CD1001", "Type should not be partial", "Formatting", "Decuplr.Bi", DiagnosticSeverity.Error, true);
        context.ReportDiagnostic(Diagnostic.Create(report, candidateClass.Modifiers.Where(x => x.IsKind(SyntaxKind.PartialKeyword)).First().GetLocation()));
    }
    */
}
