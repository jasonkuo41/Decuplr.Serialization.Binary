using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Decuplr.Sourceberg.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Sourceberg.Generation {
    internal struct InternalGeneratingContext {
        public ITypeSymbolProvider SymbolProvider { get; set; }
        public ISourceAddition SourceAddition { get; }
        public IContextCollectionProvider ContextCollection { get; }
    }

    public struct GeneratingContext {
        public SyntaxNode Node { get; }

        public SemanticModel SemanticModel { get; }

        public ImmutableArray<AdditionalText> AdditionalFiles { get; }

        public CancellationToken CancellationToken { get; }
    }

    public abstract class SourceGeneratorPath {

        internal InternalGeneratingContext Context { get; set; }

        protected ITypeSymbolCollection SourceSymbols => Context.SymbolProvider.Source;

        protected ITypeSymbolCollection CurrentSymbols => Context.SymbolProvider.Current;

        protected Compilation SourceCompilation => SourceSymbols.DeclaringCompilation;

        protected Compilation CurrentCompilation => CurrentSymbols.DeclaringCompilation;

        protected IContextCollection GetContextCollection(ISymbol symbol) => Context.ContextCollection.GetContextCollection(symbol);

        protected IContextCollection GetContextCollection(SyntaxNode node) => Context.ContextCollection.GetContextCollection(node);

        protected void AddSource(string hintName, string sourceFile) => AddSource(hintName, SourceText.From(sourceFile, Encoding.UTF8));

        protected void AddSource(string hintName, SourceText sourceText) => Context.SourceAddition.AddSource(hintName, sourceText);

        public abstract void ShouldGenerate(SyntaxNode node);

        public abstract void GenerateContext(GeneratingContext context);
    }
}
