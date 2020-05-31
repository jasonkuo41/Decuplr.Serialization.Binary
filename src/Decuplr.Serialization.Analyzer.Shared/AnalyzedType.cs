using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.Analyzers {

    // Represents an analyzed type
    public class AnalyzedType {

        private readonly Compilation Compilation;
        private readonly SourceCodeAnalyzer Analyzer;
        private readonly CancellationToken CancellationToken;
        private readonly List<AnalyzedPartialType> _declartions = new List<AnalyzedPartialType>();

        /// <summary>
        /// If the class is partial or not
        /// </summary>
        public bool IsPartial { get; }

        /// <summary>
        /// The Symbol that represents the type
        /// </summary>
        public INamedTypeSymbol TypeSymbol { get; }

        /// <summary>
        /// All the declarations found for this type, would be more then one if partial
        /// </summary>
        public IReadOnlyList<AnalyzedPartialType> Declarations => _declartions;

        public bool ContainsAttribute<T>(SymbolEqualityComparer? comparer = null) where T : Attribute {
            return TypeSymbol.GetAttributes().Any(x => x.AttributeClass?.Equals(Analyzer.GetSymbol<T>(), comparer ?? SymbolEqualityComparer.Default) ?? false);
        }

        public IEnumerable<AnalyzedAttribute> GetAttributes<T>(SymbolEqualityComparer? comparer = null) where T : Attribute {
            return Declarations.SelectMany(x => x.Attributes.SelectMany(x => x)).Where(x => x.Data.AttributeClass?.Equals(Analyzer.GetSymbol<T>(), comparer ?? SymbolEqualityComparer.Default) ?? false);
        }

        /// <summary>
        /// Adds existing declaration syntax to the info
        /// </summary>
        internal void AddSyntax(TypeDeclarationSyntax typeSyntax) {
            var model = Compilation.GetSemanticModel(typeSyntax.SyntaxTree, true);
            if (!(model.GetDeclaredSymbol(typeSyntax, CancellationToken)?.Equals(TypeSymbol, SymbolEqualityComparer.Default) ?? false))
                throw new ArgumentException($"Syntax is not type of {TypeSymbol}");
            AddSyntaxUnsafe(typeSyntax, model);
        }

        internal void AddSyntaxUnsafe(TypeDeclarationSyntax declarationSyntax, SemanticModel model) {
            _declartions.Add(new AnalyzedPartialType(this, declarationSyntax, Analyzer, model));
        }

        internal AnalyzedType(SourceCodeAnalyzer analyzer, TypeDeclarationSyntax declarationSyntax, INamedTypeSymbol symbol, Compilation compilation, SemanticModel semanticModel, CancellationToken cancellationToken) {
            IsPartial = declarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
            Analyzer = analyzer;
            Compilation = compilation;
            CancellationToken = cancellationToken;
            TypeSymbol = symbol;
            _declartions.Add(new AnalyzedPartialType(this, declarationSyntax, analyzer, semanticModel));
        }

    }
}
