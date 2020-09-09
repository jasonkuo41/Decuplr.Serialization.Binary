using System.Collections.Generic;
using System.ComponentModel;
using Decuplr.Sourceberg.Generation;
using Decuplr.Sourceberg.Services;
using Decuplr.Sourceberg.Services.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Sourceberg.Internal {

    /// <summary>
    /// A helper class to setup the analyzer
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SourcebergAnalyzer {

        private readonly IReadOnlyList<AnalyzerGroupInfo<ISymbol, SymbolKind>> symbolActionGroups;
        private readonly IReadOnlyList<AnalyzerGroupInfo<SyntaxNode, SyntaxKind>> syntaxActionGroups;
        private readonly ServiceCollection _services = new ServiceCollection();

        public SourcebergAnalyzer(GeneratorStartup startup) {
            var syntaxActionSetup = new SyntaxAnalyzerSetup();
            var symbolActionSetup = new SymbolAnalyzerGroupSetup();
            startup.ConfigureAnalyzer(new AnalyzerSetupContext(syntaxActionSetup, symbolActionSetup));
            startup.ConfigureServices(_services);
            syntaxActionGroups = syntaxActionSetup.AnalyzerGroups;
            symbolActionGroups = symbolActionSetup.AnalyzerGroups;

            foreach(var (actionGroup, _) in syntaxActionGroups) {
                actionGroup.RegisterServices(_services);
            }
            foreach(var (actionGroup, _) in symbolActionGroups) {
                actionGroup.RegisterServices(_services);
            }

            // Add default services
            _services.AddScopedGroup<SourceContextAccessor, IAnalysisLifetime>();
            _services.AddScopedGroup<TypeSymbolProvider, ITypeSymbolProvider, ISourceAddition>();

            _services.AddScopedGroup<AttributeLayoutProvider, IAttributeLayoutProvider>();
            _services.AddScopedGroup<ContextCollectionProvider, IContextCollectionProvider>();
            _services.AddScoped<TypeSymbolLocatorCache>();
        }

        public void Build(AnalysisContext context) {
            var serviceProvider = _services.BuildServiceProvider();

            context.EnableConcurrentExecution();
            // Currently we don't support popping analysis on generated code
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            foreach (var (symbolAction, requestKinds) in symbolActionGroups) {
                context.RegisterSymbolAction(symbolContext => symbolAction.InvokeScope(serviceProvider, symbolContext.Symbol, symbolContext.Compilation, symbolContext.CancellationToken), requestKinds);
            }
            foreach (var (syntaxAction, requestKinds) in syntaxActionGroups) {
                context.RegisterSyntaxNodeAction(syntaxContext => syntaxAction.InvokeScope(serviceProvider, syntaxContext.Node, syntaxContext.Compilation, syntaxContext.CancellationToken), requestKinds);
            }

        }

    }

}
