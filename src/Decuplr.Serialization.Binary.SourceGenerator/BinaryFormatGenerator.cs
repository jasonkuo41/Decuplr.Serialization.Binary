using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Decuplr.Serialization.Binary.Analyzers;
using Decuplr.Serialization.Binary.SourceGenerator.BinaryFormatSource;
using Decuplr.Serialization.Binary.SourceGenerator.ParserSource;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator {

    [Generator]
    public class BinaryFormatGenerator : ISourceGenerator {
        private class CandidateSyntaxReceiver : ISyntaxReceiver {

            public List<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
                // We capture every class we are interested in
                // We only capture classes that comes with attribute, but we may also be interested in those with specific syntax ending
                if (syntaxNode is TypeDeclarationSyntax classSyntax && classSyntax.AttributeLists.Any()) {
                    CandidateTypes.Add(classSyntax);
                }
                // TODO : Add interfaces, note we may only allow interfaces with public setters
            }
        }

        public void Initialize(InitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new CandidateSyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context) {
            if (!(context.SyntaxReceiver is CandidateSyntaxReceiver receiver))
                return;
            var sourceBuilder = new StringBuilder(@"using System;
                namespace Decuplr.Serialization.Binary {
                    public static class DebugContent {
                        public static void PrintDebugInfo() {
                            Console.WriteLine(""Start of debug info"");
                ");

            try {
                // We also need to dump struct output (markdown file) for output
                var generationSources = new IParserGenerateSource[] {
                    // Responsible for [BinaryFormat] tags
                    new BinaryFormatSG(),
                    // Responsible for [BinaryParser] tags
                    new BinaryParserSG()
                };

                var types = SourceCodeAnalyzer.AnalyzeTypeSyntax(receiver.CandidateTypes, context.Compilation, context.CancellationToken);

                var generatedResults = new List<GeneratedParser>();

                // We loop through all the internal generators
                foreach (var source in generationSources) {
                    var generated = source.GenerateParser(types, context);
                    foreach (var additionalFile in generated.AdditionalFiles)
                        context.AddSource(additionalFile);
                    generatedResults.AddRange(generated.GeneratedParser);
                }
                context.AddSource(BinaryPackerEntryPointGenerator.CreateSourceText(generatedResults));

            }
            catch (Exception e) {
                sourceBuilder.AppendLine($"Console.WriteLine(@\"{e} {e.Message} \r \n {e.StackTrace}\");");
            }

            sourceBuilder.Append(@"} } }");
            context.AddSource("DebugInfo.BinaryFormatter.Generated.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));

        }

    }

}
