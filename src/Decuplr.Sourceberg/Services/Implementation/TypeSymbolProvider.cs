using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Sourceberg.Services.Implementation {
    internal class TypeSymbolProvider : ITypeSymbolProvider, ISourceAddition {

        private class SourceSink : ITypeSymbolCollection {

            private Compilation? compilation;
            private ReflectionTypeSymbolLocator? symbolLocator;

            protected TypeSymbolProvider Parent { get; }

            public Compilation DeclaringCompilation {
                get {
                    EnsureCompilation();
                    Debug.Assert(compilation is { });
                    return compilation;
                }
                protected set {
                    compilation = value;
                    symbolLocator = Parent._locatorCache.GetLocator(compilation);
                }
            }

            public ReflectionTypeSymbolLocator SymbolLocator {
                get {
                    EnsureCompilation();
                    Debug.Assert(symbolLocator is { });
                    return symbolLocator;
                }
            }

            private void EnsureCompilation() {
                if (compilation is { })
                    return;
                compilation ??= Parent?._context.SourceCompilation ?? throw new NotSupportedException("Compilation is not supported");
                symbolLocator = Parent._locatorCache.GetLocator(compilation);
            }

            public SourceSink(TypeSymbolProvider provider) => Parent = provider;

            public ITypeSymbol? GetSymbol<T>() => GetSymbol(typeof(T));

            public ITypeSymbol? GetSymbol(Type type) => SymbolLocator.GetTypeSymbol(type);

        }

        private class CurrentSourceSink : SourceSink {

            public List<GeneratedSourceText> GeneratedSourceTexts { get; } = new List<GeneratedSourceText>();

            public CurrentSourceSink(TypeSymbolProvider provider) : base(provider) { }

            public SyntaxTree AddSource(string fileName, string sourceCode) 
                => AddSource(new GeneratedSourceText(fileName, SourceText.From(sourceCode, encoding: Encoding.UTF8)));

            public SyntaxTree AddSource(GeneratedSourceText sourceText) {
                var compilation = DeclaringCompilation as CSharpCompilation ?? throw new NotSupportedException("Non C# compilation is not supported");
                var syntax = CSharpSyntaxTree.ParseText(sourceText.Text, new CSharpParseOptions(compilation.LanguageVersion), isGeneratedCode: true, cancellationToken: Parent._context.OnOperationCanceled);
                DeclaringCompilation = DeclaringCompilation.AddSyntaxTrees(syntax);
                GeneratedSourceTexts.Add(sourceText);
                return syntax;
            }

        }

        private readonly SourceContextAccessor _context;
        private readonly CurrentSourceSink _currentSink;
        private readonly TypeSymbolLocatorCache _locatorCache;

        public IReadOnlyList<GeneratedSourceText> GeneratedSourceTexts => _currentSink.GeneratedSourceTexts;

        public ITypeSymbolCollection Current => _currentSink;

        public ITypeSymbolCollection Source { get; }

        public TypeSymbolProvider(SourceContextAccessor accessor, TypeSymbolLocatorCache locatorCache) {
            _locatorCache = locatorCache;
            _context = accessor;
            _currentSink = new CurrentSourceSink(this);
            Source  = new SourceSink(this);
        }

        public SyntaxTree AddSource(string fileName, string sourceCode) => _currentSink.AddSource(fileName, sourceCode);
        public SyntaxTree AddSource(GeneratedSourceText sourceText) => _currentSink.AddSource(sourceText);
    }
}
