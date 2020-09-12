using System;
using System.Collections.Generic;
using System.ComponentModel;
using Decuplr.Sourceberg.Generation;
using Decuplr.Sourceberg.Services;
using Decuplr.Sourceberg.Services.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Sourceberg.Internal {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class SourcebergBase {

        private readonly ServiceCollection _services = new ServiceCollection();

        private protected IReadOnlyList<AnalyzerGroupInfo<ISymbol, SymbolAnalysisContextSource, SymbolKind>> SymbolActionGroups { get; }
        private protected IReadOnlyList<AnalyzerGroupInfo<SyntaxNode, SyntaxNodeAnalysisContextSource, SyntaxKind>> SyntaxActionGroups { get; }

        private protected SourcebergBase(GeneratorStartup startup) {
            var syntaxActionSetup = new SyntaxAnalyzerSetup();
            var symbolActionSetup = new SymbolAnalyzerGroupSetup();
            startup.ConfigureAnalyzer(new AnalyzerSetupContext(syntaxActionSetup, symbolActionSetup));
            startup.ConfigureServices(_services);
            SyntaxActionGroups = syntaxActionSetup.AnalyzerGroups;
            SymbolActionGroups = symbolActionSetup.AnalyzerGroups;

            foreach (var (actionGroup, _) in SyntaxActionGroups) {
                actionGroup.RegisterServices(_services);
            }
            foreach (var (actionGroup, _) in SymbolActionGroups) {
                actionGroup.RegisterServices(_services);
            }

            // Add default services
            _services.AddScopedGroup<SourceContextAccessor, IAnalysisLifetime>();
            _services.AddScopedGroup<TypeSymbolProvider, ITypeSymbolProvider, ISourceAddition>();

            _services.AddScopedGroup<AttributeLayoutProvider, IAttributeLayoutProvider>();
            _services.AddScopedGroup<ContextCollectionProvider, IContextCollectionProvider>();
            _services.AddScoped<TypeSymbolLocatorCache>();
        }

        protected IServiceProvider CreateServiceProvider() => _services.BuildServiceProvider();


    }

}
