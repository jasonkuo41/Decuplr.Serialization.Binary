using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Decuplr.Sourceberg.Generation;
using Decuplr.Sourceberg.Services;
using Decuplr.Sourceberg.Services.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Decuplr.Sourceberg.Internal {

    public class SourcebergBase {

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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class SyntaxCapture : ISyntaxReceiver {
        private readonly List<SyntaxNode> _nodes = new List<SyntaxNode>();

        internal IReadOnlyList<SyntaxNode> CapturedNodes => _nodes;
        internal GeneratorStartup Startup { get; }

        internal SyntaxCapture(GeneratorStartup startup) => Startup = startup;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (Startup.ShouldCapture(syntaxNode))
                _nodes.Add(syntaxNode);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class SourcebergGenerator : SourcebergBase {

        private readonly SourceGeneratorContext _generatorContext;
        private readonly SyntaxCapture _capture;
        private readonly Dictionary<SyntaxTree, SemanticModel> _semanticModelCache = new Dictionary<SyntaxTree, SemanticModel>();

        private SourcebergGenerator(SourceGeneratorContext generatorContext, SyntaxCapture capture)
            : base(capture.Startup) {
            _generatorContext = generatorContext;
            _capture = capture;
        }

        private SemanticModel GetSemanticModel(SyntaxNode node) {
            return _semanticModelCache.GetOrAdd(node.SyntaxTree, tree => _generatorContext.Compilation.GetSemanticModel(tree));
        }

        private IEnumerable<IEnumerable<Diagnostic>> RunSyntaxAnalyzerGroup(IServiceProvider serviceProvider, SyntaxNode syntaxNode, IContextCollectionProvider externalContextProvider) {
            return SyntaxActionGroups.Where(x => x.SelectedEnumKinds.Contains(syntaxNode.Kind()))
                                     .Select(x => x.Group.InvokeScope(serviceProvider,
                                                                      syntaxNode,
                                                                      _generatorContext.Compilation,
                                                                      GetNextContext,
                                                                      externalContextProvider,
                                                                      _generatorContext.CancellationToken));
            
            SyntaxNodeAnalysisContextSource GetNextContext(SyntaxNode nextSyntax, bool isEntryPoint)
                => new SyntaxNodeAnalysisContextSource(nextSyntax,
                                                       GetSemanticModel(nextSyntax),
                                                       _generatorContext.Compilation,
                                                       _generatorContext.AdditionalFiles,
                                                       _generatorContext.AnalyzerConfigOptions,
                                                       _generatorContext.CancellationToken);
        }

        private IEnumerable<IEnumerable<Diagnostic>> RunSymbolAnalyzerGroup(IServiceProvider serviceProvider, SyntaxNode syntaxNode, IContextCollectionProvider externalContextProvider) {
            foreach (var (symbolAction, requestKinds) in SymbolActionGroups) {
                var semanticModel = GetSemanticModel(syntaxNode);
                var symbol = semanticModel.GetDeclaredSymbol(syntaxNode, _generatorContext.CancellationToken)
                             ?? semanticModel.GetSymbolInfo(syntaxNode, _generatorContext.CancellationToken).Symbol;
                if (symbol is null)
                    continue;
                if (!requestKinds.Contains(symbol.Kind))
                    continue;
                yield return symbolAction.InvokeScope(serviceProvider,
                                                      symbol,
                                                      _generatorContext.Compilation,
                                                      GetNextContext,
                                                      externalContextProvider,
                                                      _generatorContext.CancellationToken);

                SymbolAnalysisContextSource GetNextContext(ISymbol symbol, bool isEntryPoint)
                    => new SymbolAnalysisContextSource(symbol,
                                                       _generatorContext.Compilation,
                                                       _generatorContext.AdditionalFiles,
                                                       _generatorContext.AnalyzerConfigOptions,
                                                       _generatorContext.CancellationToken);
            }
        }

        // return true if we should continue
        private bool ReportDiagnostics(IEnumerable<Diagnostic> diagnostics) {
            bool hasError = false;
            foreach (var result in diagnostics) {
                hasError |= result.Severity == DiagnosticSeverity.Error;
                _generatorContext.ReportDiagnostic(result);
            }
            return hasError;
        }

        public void RunGeneration() {
            using var scope = CreateServiceProvider().CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var externalContextProvider = serviceProvider.GetRequiredService<IContextCollectionProvider>();

            // First we validate all the captured syntax are valid
            foreach (var syntaxNode in _capture.CapturedNodes) {
                // We rely on lazy evaluation so that if the first group fails, we just short cut out
                var diagnosticsGroup = RunSyntaxAnalyzerGroup(serviceProvider, syntaxNode, externalContextProvider);
                diagnosticsGroup = diagnosticsGroup.Concat(RunSymbolAnalyzerGroup(serviceProvider, syntaxNode, externalContextProvider));

                foreach (var diagnostics in diagnosticsGroup) {
                    if (ReportDiagnostics(diagnostics))
                        return;
                }
            }

            // Then finally we run the generation
            // TODO : Handle Exceptions
            try {
                var generators = serviceProvider.GetServices<SourceGenerator>();
                foreach (var generator in generators) {
                    generator.Context = new InternalGeneratingContext {
                        ContextCollectionSource = externalContextProvider,
                        SourceAddition = serviceProvider.GetRequiredService<ISourceAddition>(),
                        SymbolProvider = serviceProvider.GetRequiredService<ITypeSymbolProvider>()
                    };
                    foreach (var syntaxNode in _capture.CapturedNodes) {
                        var context = new GenerationContext(syntaxNode,
                                                            GetSemanticModel(syntaxNode),
                                                            _generatorContext.AdditionalFiles,
                                                            _generatorContext.AnalyzerConfigOptions,
                                                            _generatorContext.CancellationToken);
                        generator.GenerateContext(context);
                    }
                    foreach (var source in generator.GeneratedSource) {
                        _generatorContext.AddSource(source.HintName, source.SourceText);
                    }
                }
            } 
            catch(Exception e) {
                // Catch exception and report diagnostic
                "Report Diagnostic"
            }
        }

        internal static SourcebergGenerator? CreateGenerator(SourceGeneratorContext context) {
            if (context.SyntaxReceiver is SyntaxCapture syntaxCapture)
                return new SourcebergGenerator(context, syntaxCapture);
            return null;
        }
    }

}
