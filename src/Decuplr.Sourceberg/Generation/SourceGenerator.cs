using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Decuplr.Sourceberg.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Sourceberg.Generation {
    internal struct InternalGeneratingContext {
        public ITypeSymbolProvider SymbolProvider { get; set; }
        public ISourceAddition SourceAddition { get; set; }
        public IContextCollectionProvider ContextCollectionSource { get; set; }
    }

    public struct GenerationContext {
        public GenerationContext(SyntaxNode node, SemanticModel semanticModel, ImmutableArray<AdditionalText> additionalFiles, AnalyzerConfigOptionsProvider analyzerConfigOptions, CancellationToken cancellationToken) {
            Node = node;
            SemanticModel = semanticModel;
            AdditionalFiles = additionalFiles;
            AnalyzerConfigOptions = analyzerConfigOptions;
            CancellationToken = cancellationToken;
        }

        public SyntaxNode Node { get; }

        public SemanticModel SemanticModel { get; }

        public ImmutableArray<AdditionalText> AdditionalFiles { get; }

        public AnalyzerConfigOptionsProvider AnalyzerConfigOptions { get; }

        public CancellationToken CancellationToken { get; }
    }

    internal struct GeneratedSourceText {
        public string HintName { get; }
        public SourceText SourceText { get; }
        public GeneratedSourceText(string hintName, SourceText sourceText) {
            HintName = hintName;
            SourceText = sourceText;
        }
    }

    public abstract class SourceGenerator {

        private readonly List<GeneratedSourceText> _generatedSource = new List<GeneratedSourceText>();

        internal InternalGeneratingContext Context { get; set; }

        internal IReadOnlyList<GeneratedSourceText> GeneratedSource => _generatedSource;

        protected ITypeSymbolCollection SourceSymbols => Context.SymbolProvider.Source;

        protected ITypeSymbolCollection CurrentSymbols => Context.SymbolProvider.Current;

        protected Compilation SourceCompilation => SourceSymbols.DeclaringCompilation;

        protected Compilation CurrentCompilation => CurrentSymbols.DeclaringCompilation;

        protected IContextCollection GetContextCollection(ISymbol symbol) => Context.ContextCollectionSource.GetContextCollection(symbol);

        protected IContextCollection GetContextCollection(SyntaxNode node) => Context.ContextCollectionSource.GetContextCollection(node);

        protected void AddSource(string hintName, string sourceFile) => AddSource(hintName, SourceText.From(sourceFile, Encoding.UTF8));

        protected void AddSource(string hintName, SourceText sourceText) {
            Context.SourceAddition.AddSource(hintName, sourceText);
            _generatedSource.Add(new GeneratedSourceText(hintName, sourceText));
        }

        public abstract void GenerateContext(GenerationContext context);
    }
}
