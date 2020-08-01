using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class TypeSymbolCollection : ITypeSymbolProvider, ISourceAddition {

        private static Exception NotSupported { get; } = new NotSupportedException("Type is not found with the current compilation");

        private readonly CancellationToken _ct;

        private readonly Dictionary<Type, INamedTypeSymbol> _cachedSymbols = new Dictionary<Type, INamedTypeSymbol>();
        private readonly CSharpCompilation _sourceCompilation;
        private CSharpCompilation _currentCompilation;

        public event EventHandler<GeneratedSourceText>? OnSourceGenerated;

        public TypeSymbolCollection(ICompilationInfo info, ICompilationLifetime lifetime) {
            _sourceCompilation = info.SourceCompilation as CSharpCompilation ?? throw new NotSupportedException("Source Generator does not support languages other then C#");
            _currentCompilation = _sourceCompilation;
            _ct = lifetime.OnCompilationCancelled;
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

        public void AddSource(string fileName, string sourceCode) => AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));
        public void AddSource(string fileName, SourceText text) => AddSource(new GeneratedSourceText(fileName, text));
        public void AddSource(GeneratedSourceText sourceText) {
            var syntax = CSharpSyntaxTree.ParseText(sourceText.Text, new CSharpParseOptions(_sourceCompilation.LanguageVersion), "", null, true, _ct);
            _currentCompilation = _currentCompilation.AddSyntaxTrees(syntax);
            OnSourceGenerated?.Invoke(this, sourceText);
        }
    }

}
