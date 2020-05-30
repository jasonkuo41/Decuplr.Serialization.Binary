﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Decuplr.Serialization.Binary.SourceGenerator.Solutions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                sourceBuilder.AppendLine($"Console.WriteLine(\"   {candidateClass.AttributeLists[0].Attributes.Count}\");");
                candidateSymbols.Add(typeSymbol!);
            }

            try {

                var result = new List<TypeParserGenerator>();
                foreach (var typeSymbol in candidateSymbols) {
                    sourceBuilder.AppendLine($"Console.WriteLine(\"{typeSymbol}\");");
                    if (!TypeInfoDiscovery.TryParseType(typeSymbol, context, out var typeInfo))
                        continue;

                    var serializer = new PartialTypeSerialize(typeInfo!);
                    var deserializer = new PartialTypeDeserialize(typeInfo!);

                    result.Add(new TypeParserGenerator(typeInfo!, deserializer, serializer));
                }

                // Generate an entry point
                //context.AddSource(EntryPointBuilder.CreateSourceText(result));

                // Generate those additional files
                //foreach (var additionFile in result.SelectMany(x => x.AdditionalCode))
                //    context.AddSource(additionFile);
            }
            catch (Exception e) {
                sourceBuilder.AppendLine($"Console.WriteLine(@\"{e} {e.Message} \r \n {e.StackTrace}\");");
            }

            sourceBuilder.Append(@"} } }");
            context.AddSource("DebugInfo.BinaryFormatter.Generated.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));

        }

    }

}
