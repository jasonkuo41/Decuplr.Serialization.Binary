using System;
using System.Collections.Generic;
using System.Threading;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.Analyzers {
    /// <summary>
    /// Represents a analyzer that caputers and analyze info we want to know
    /// </summary>
    public class SourceCodeAnalyzer {

        private readonly Compilation Compilation;
        private readonly Dictionary<Type, INamedTypeSymbol?> CachedSymbols = new Dictionary<Type, INamedTypeSymbol?>();

        public INamedTypeSymbol? GetSymbol<T>() => GetSymbol(typeof(T));

        public INamedTypeSymbol? GetSymbol(Type type) {
            if (CachedSymbols.TryGetValue(type, out var symbol))
                return symbol;
            symbol = Compilation.GetTypeByMetadataName(type.FullName);
            CachedSymbols.Add(type, symbol);
            return symbol;
        }

        public SourceCodeAnalyzer(Compilation compilation) {
            Compilation = compilation;
        }

        public static IEnumerable<AnalyzedType> AnalyzeTypeSyntax(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, CancellationToken ct) {
            var analyzer = new SourceCodeAnalyzer(compilation);
            var types = new Dictionary<INamedTypeSymbol, AnalyzedType>();
            foreach(var syntax in declarationSyntaxes) {
                var model = compilation.GetSemanticModel(syntax.SyntaxTree, true);
                var typeSymbol = model.GetDeclaredSymbol(syntax, ct);
                if (typeSymbol is null)
                    continue;
                if (!types.TryGetValue(typeSymbol, out var analyzed)) {
                    var analyzedType = new AnalyzedType(analyzer, syntax, typeSymbol, compilation, model, ct);
                    types.Add(typeSymbol, analyzedType);
                    yield return analyzedType;
                    continue;
                }

                analyzed.AddSyntaxUnsafe(syntax, model);
            }
        }
    }
}
