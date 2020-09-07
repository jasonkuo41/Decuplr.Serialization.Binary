using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Text;
using Decuplr.Sourceberg.Generation;
using Decuplr.Sourceberg.Services;
using Decuplr.Sourceberg.Services.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Sourceberg.Internal {



    internal class SyntaxAnalyzerSetup : ISyntaxAnalyzerSetup {

        private class SyntaxAnalyzerThenSetup<TSyntax> : ISyntaxAnalyzerSetupGroup<TSyntax> where TSyntax : SyntaxNode {
            public ISyntaxAnalyzerSetupGroup<TSyntax> ThenUseAnalyzer<TAnalyzer>() where TAnalyzer : SyntaxNodeAnalyzer<TSyntax> {
                throw new NotImplementedException();
            }

            public ISyntaxAnalyzerSetupGroup<TNewSyntax> ThenUseAnalyzer<TAnalyzer, TNewSyntax>(Func<TSyntax, TNewSyntax> syntaxFactory) where TNewSyntax : SyntaxNode {
                throw new NotImplementedException();
            }

            public ISyntaxAnalyzerSetupGroup<TNewSyntax> ThenUseAnalyzer<TAnalyzer, TNewSyntax>(Func<TSyntax, IEnumerable<TNewSyntax>> syntaxFactory) where TNewSyntax : SyntaxNode {
                throw new NotImplementedException();
            }
        }

        private readonly List<Type> _analyzers = new List<Type>();

        public IReadOnlyList<Type> Analyzers => _analyzers;

        public SyntaxKind SyntaxKind { get; }

        public Type SyntaxType { get; }

        public ISyntaxAnalyzerSetupGroup<TSyntax> UseAnalyzer<TAnalyzer, TSyntax>()
            where TAnalyzer : SyntaxNodeAnalyzer<TSyntax>
            where TSyntax : SyntaxNode {

            _analyzers.Add(typeof(TAnalyzer));
            
        }
    }

    internal class SymbolAnalyzerSetup : ISymbolAnalyzerSetup { }

    internal readonly struct AnalzyerGroupContext<TContext, TKind> {
        public Action<IServiceProvider, TContext> SymbolAction { get; }
        public ImmutableArray<TKind> SymbolKinds { get; }
        public AnalzyerGroupContext(Action<IServiceProvider, TContext> symbolAction, ImmutableArray<TKind> symbolKinds) {
            SymbolAction = symbolAction;
            SymbolKinds = symbolKinds;
        }

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SourcebergAnalyzer {

        private readonly IReadOnlyList<AnalzyerGroupContext<SymbolAnalysisContext, SymbolKind>> symbolActionGroups;
        private readonly IReadOnlyList<AnalzyerGroupContext<SyntaxNodeAnalysisContext, SyntaxKind>> syntaxActionGroups;
        private readonly ServiceCollection _services = new ServiceCollection();

        public SourcebergAnalyzer(GeneratorStartup startup) {
            var syntaxActionSetup = new SyntaxAnalyzerSetup();
            var symbolActionSetup = new SymbolAnalyzerSetup();
            startup.ConfigureAnalyzer(new AnalyzerSetupContext(syntaxActionSetup, symbolActionSetup));
            startup.ConfigureServices(_services);
        }

        private void AddDefaultServices() {
            _services.AddScopedGroup<SourceContextAccessor, IAnalysisLifetime>();
            _services.AddScopedGroup<TypeSymbolProvider, ITypeSymbolProvider, ISourceAddition>();

            _services.AddScopedGroup<AttributeLayoutProvider, IAttributeLayoutProvider>();
            _services.AddScopedGroup<ContextCollectionProvider, IContextCollectionProvider>();
            _services.AddScoped<TypeSymbolLocatorCache>();

        }

        public void Build(AnalysisContext context) {
            AddDefaultServices();
            var serviceProvider = _services.BuildServiceProvider();

            context.EnableConcurrentExecution();
            // Currently we don't support popping analysis on generated code
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            foreach (var symbolAction in symbolActionGroups) {
                context.RegisterSymbolAction(symbolContext => symbolAction.SymbolAction(serviceProvider, symbolContext), symbolAction.SymbolKinds);
            }
            foreach (var syntaxAction in syntaxActionGroups) {
                context.RegisterSyntaxNodeAction(syntaxContext => syntaxAction.SymbolAction(serviceProvider, syntaxContext), syntaxAction.SymbolKinds);
            }
        }

    }

    public struct AnalysisActions {

        IEnumerable<SymbolKind> SymbolKinds { get; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SourcebergGenerator {

        internal SourcebergGenerator(IServiceProvider serviceProvider) {

        }

        public void RunGeneration(SourceGeneratorContext generatorContext) {

        }

    }

}
