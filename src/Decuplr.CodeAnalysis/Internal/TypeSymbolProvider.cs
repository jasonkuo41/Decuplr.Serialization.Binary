using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.CodeAnalysis.Internal {
    internal class TypeSymbolProvider : ITypeSymbolProvider {

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

}
