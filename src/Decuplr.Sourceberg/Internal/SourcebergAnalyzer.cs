﻿using System.Collections.Generic;
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

        private readonly IReadOnlyList<AnalyzerGroupInfo<ISymbol, SymbolAnalysisContextSource, SymbolKind>> symbolActionGroups;
        private readonly IReadOnlyList<AnalyzerGroupInfo<SyntaxNode, SyntaxNodeAnalysisContextSource, SyntaxKind>> syntaxActionGroups;
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
                context.RegisterSymbolAction(symbolContext => {
                    var diagnostics = symbolAction.InvokeScope(serviceProvider,
                                                               symbolContext.Symbol,
                                                               symbolContext.Compilation,
                                                               (nextSymbol, _) => SymbolAnalysisContextSource.FromContextSource(nextSymbol, symbolContext),
                                                               symbolContext.CancellationToken);
                    foreach (var diagnostic in diagnostics)
                        symbolContext.ReportDiagnostic(diagnostic);
                }, requestKinds);
            }
            foreach (var (syntaxAction, requestKinds) in syntaxActionGroups) {
                context.RegisterSyntaxNodeAction(syntaxContext => {
                    var compilation = syntaxContext.Compilation;
                    var ct = syntaxContext.CancellationToken;

                    var reportedDiagnostics = syntaxAction.InvokeScope(serviceProvider, syntaxContext.Node, compilation, GetNextContext, ct);

                    foreach (var reportedDiagnostic in reportedDiagnostics)
                        syntaxContext.ReportDiagnostic(reportedDiagnostic);

                    SyntaxNodeAnalysisContextSource GetNextContext(SyntaxNode nextSyntax, bool isEntryPoint) 
                        => isEntryPoint
                           ? SyntaxNodeAnalysisContextSource.FromContextSource(syntaxContext)
                           : SyntaxNodeAnalysisContextSource.FromInherit(nextSyntax, syntaxContext);

                }, requestKinds);
            }

        }

    }

}
