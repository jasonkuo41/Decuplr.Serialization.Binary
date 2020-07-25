using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Serialization.AnalysisService {

    public class TypeSymbolProvider : ITypeSymbolProvider {

        private static Exception NotSupported { get; } = new NotSupportedException("Type is not found with the current compilation");

        private readonly CancellationToken _ct;
        private readonly Dictionary<Type, INamedTypeSymbol> _cachedSymbols = new Dictionary<Type, INamedTypeSymbol>();
        private readonly CSharpCompilation _sourceCompilation;
        private CSharpCompilation _currentCompilation;

        public TypeSymbolProvider(Compilation compilation, CancellationToken ct) {
            _sourceCompilation = compilation as CSharpCompilation ?? throw new NotSupportedException("Source Generator does not support languages other then C#");
            _currentCompilation = _sourceCompilation;
            _ct = ct;
        }

        public void AddSource(string sourceCode) {
            var sourceText = SourceText.From(sourceCode, Encoding.UTF8);
            var syntax = CSharpSyntaxTree.ParseText(sourceText, new CSharpParseOptions(_sourceCompilation.LanguageVersion), "", null, true, _ct);
            _currentCompilation = _currentCompilation.AddSyntaxTrees(syntax);
        }

        public INamedTypeSymbol GetSymbol<T>() => GetSymbol(typeof(T));

        public INamedTypeSymbol GetSymbol(Type type) {
            if (TryGetSymbol(type, out var symbol))
                return symbol!;
            throw NotSupported;
        }

        public INamedTypeSymbol GetSymbol(string fullName) {
            if (TryGetSymbol(fullName, out var symbol))
                return symbol!;
            throw NotSupported;
        }

        public bool TryGetSymbol(string fullName, out INamedTypeSymbol? symbol) {
            symbol = _currentCompilation.GetTypeByMetadataName(fullName);
            return symbol is null;
        }

        public bool TryGetSymbol<T>(out INamedTypeSymbol? symbol) => TryGetSymbol(typeof(T), out symbol);

        public bool TryGetSymbol(Type type, out INamedTypeSymbol? symbol) {
            if (_cachedSymbols.TryGetValue(type, out symbol))
                return true;
            symbol = _currentCompilation.GetTypeByMetadataName(type.FullName);
            if (symbol is null)
                return false;
            _cachedSymbols.Add(type, symbol);
            return true;
        }
    }

    public class SourceCodeAnalysis : ITypeSymbolProvider {

        private readonly Compilation Compilation;
        private readonly Dictionary<Type, INamedTypeSymbol> CachedSymbols = new Dictionary<Type, INamedTypeSymbol>();
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

        public INamedTypeSymbol GetSymbol<T>() => GetSymbol(typeof(T));

        public INamedTypeSymbol GetSymbol(Type type) {
            if (TryGetSymbol(type, out var symbol))
                return symbol!;
            throw new NotSupportedException("Type is not found with the current compilation");
        }

        public bool TryGetSymbol<T>(out INamedTypeSymbol? symbol) => TryGetSymbol(typeof(T), out symbol);

        public bool TryGetSymbol(Type type, out INamedTypeSymbol? symbol) {
            if (CachedSymbols.TryGetValue(type, out symbol))
                return true;
            symbol = Compilation.GetTypeByMetadataName(type.FullName);
            if (symbol is null)
                return false;
            CachedSymbols.Add(type, symbol);
            return true;
        }

    }

}
