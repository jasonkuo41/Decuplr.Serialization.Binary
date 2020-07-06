using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.AnalysisService {

    public class SourceCodeAnalysis {

        private readonly Compilation Compilation;
        private readonly Dictionary<Type, INamedTypeSymbol?> CachedSymbols = new Dictionary<Type, INamedTypeSymbol?>();
        private readonly List<NamedTypeMetaInfo> Types = new List<NamedTypeMetaInfo>();

        public IReadOnlyList<NamedTypeMetaInfo> ContainingTypes => Types;

        // We currently don't support partial methods since it's fairly limited, and none of our feature would utilize it
        public SourceCodeAnalysis(IEnumerable<TypeDeclarationSyntax> typeSyntaxes, Compilation compilation, CancellationToken ct, params SymbolKind[] memberKinds) {
            Compilation = compilation;
            var types = new Dictionary<INamedTypeSymbol, List<SyntaxModelPair>>();
            foreach (var syntax in typeSyntaxes) {
                var model = compilation.GetSemanticModel(syntax.SyntaxTree, true);
                var typeSymbol = model.GetDeclaredSymbol(syntax, ct);
                if (typeSymbol is null)
                    continue;
                if (types.TryGetValue(typeSymbol, out var syntaxList))
                    syntaxList.Add((syntax, model));
                else
                    types.Add(typeSymbol, new List<SyntaxModelPair>() { (syntax, model) });
            }

            foreach (var type in types) {
                Types.Add(new NamedTypeMetaInfo(this, type.Key, type.Value, memberKinds, ct));
            }
        }

        public INamedTypeSymbol? GetSymbol<T>() => GetSymbol(typeof(T));

        public INamedTypeSymbol? GetSymbol(Type type) {
            if (CachedSymbols.TryGetValue(type, out var symbol))
                return symbol;
            symbol = Compilation.GetTypeByMetadataName(type.FullName);
            CachedSymbols.Add(type, symbol);
            return symbol;
        }

    }

}
