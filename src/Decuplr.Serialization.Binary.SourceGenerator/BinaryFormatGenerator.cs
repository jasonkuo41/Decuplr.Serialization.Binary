using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Binary.Analyzers;
using Decuplr.Serialization.Binary.SourceGenerator.BinaryFormatSource;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            // We also need to dump struct output (markdown file) for output
            IParserGenerateSource[]? generationSources = new IParserGenerateSource[] {
                    // Responsible for [BinaryFormat] tags
                    new BinaryFormatSourceGen(),
                // Responsible for [BinaryParser] tags
                //new BinaryParserSG()
            };

            IEnumerable<Analyzer.BinaryFormat.AnalyzedType>? types = SourceCodeAnalyzer.AnalyzeTypeSyntax(receiver.CandidateTypes, context.Compilation, context.CancellationToken);

            List<GeneratedParser>? generatedResults = new List<GeneratedParser>();

            // We loop through all the internal generators
            foreach (IParserGenerateSource? source in generationSources) {
                // If anything fails, we would halt compilation immediately so we don't output junk
                if (!source.TryGenerateParser(types, context, out IEnumerable<GeneratedParser>? parsers))
                    return;
                generatedResults.AddRange(parsers);
            }

            foreach (GeneratedSourceCode additionalFiles in generatedResults.SelectMany(x => x.GeneratedSourceCodes))
                context.AddSource(additionalFiles);
            context.AddSource(BinaryPackerEntryPointGenerator.CreateSourceText(context.Compilation, generatedResults));
        }

    }

}
