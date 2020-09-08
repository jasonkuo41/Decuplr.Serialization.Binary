using System;
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


    internal abstract class AnalyzerGroupNode<TKind> where TKind : class {

        private AnalyzerGroupNode<TKind>? _nextNode;
        public AnalyzerGroup<TKind> Group { get; }
        
        public abstract Type AnalyzerType { get; }
        public Func<TKind, IEnumerable<TKind>>? PreviousTransisitionMultiple { get; }
        public Func<TKind, TKind>? PreviousTransisitionSingle { get; }

        protected AnalyzerGroupNode(AnalyzerGroup<TKind> parent) {
            Group = parent;
        }

        protected AnalyzerGroupNode(AnalyzerGroup<TKind> parent, Func<TKind, TKind>? transitionSingle) {
            Group = parent;
            PreviousTransisitionSingle = transitionSingle;
        }

        protected AnalyzerGroupNode(AnalyzerGroup<TKind> parent, Func<TKind, IEnumerable<TKind>>? transitionMultiple) {
            Group = parent;
            PreviousTransisitionMultiple = transitionMultiple;
        }

        public void SetNextNode(AnalyzerGroupNode<TKind> nextNode) {
            if (_nextNode is { })
                throw new InvalidOperationException("Adding more childs to a singular analyzer node is not allowed");
            _nextNode = nextNode;
            Group.Add(nextNode);
        }
    }

    internal class AnalyzerGroup<TKind> where TKind : class {

        private readonly List<AnalyzerGroupNode<TKind>> _analyzers = new List<AnalyzerGroupNode<TKind>>();
        public IReadOnlyList<AnalyzerGroupNode<TKind>> Analyzers => _analyzers;

        internal void Add(AnalyzerGroupNode<TKind> node) => _analyzers.Add(node);

        private IReadOnlyList<IReadOnlyList<TKind>> ExpandKindForInvocation(TKind origin){
            var kindList = new List<TKind>[Analyzers.Count];
            for (int i = 0; i < Analyzers.Count; i++) {
                var analyzerInfo = Analyzers[i];
                if (i == 0) {
                    kindList[i] = new List<TKind> { origin };
                    continue;
                }
                var previousKinds = kindList[i - 1];
                if (analyzerInfo.PreviousTransisitionSingle is { }) {
                    kindList[i] = new List<TKind>(previousKinds.Count);
                    for (var j = 0; j < previousKinds.Count; ++j)
                        kindList[i].Add(analyzerInfo.PreviousTransisitionSingle(previousKinds[j]));
                    continue;
                }
                if (analyzerInfo.PreviousTransisitionMultiple is { }) {
                    kindList[i] = kindList[i - 1].SelectMany(x => analyzerInfo?.PreviousTransisitionMultiple(x)).ToList();
                    continue;
                }
                kindList[i] = kindList[i - 1];
                continue;
            }
            return kindList;
        }

        public void InvokeScope(IServiceProvider service, TKind kind, Compilation? compilation, CancellationToken ct) {
            using var scope = service.CreateScope();
            var scopeService = scope.ServiceProvider;
            var accessor = scopeService.GetRequiredService<SourceContextAccessor>();
            {
                accessor.OnOperationCanceled = ct;
                accessor.SourceCompilation = compilation;
            }
            var contextProvider = scopeService.GetRequiredService<IContextCollectionProvider>();

            var kindList = ExpandKindForInvocation(kind);

            // reverse iterator
            Action<CancellationToken> prevNextAction = ct => { };
            for(var i = Analyzers.Count - 1; i >=0; --i) {
                var analyzerInfo = Analyzers[i];
                var analyzer = (SourceAnalyzerBase)scopeService.GetRequiredService(analyzerInfo.AnalyzerType);

                void NextAction(CancellationToken providedCt) {
                    for (int j = 0; j < kindList[i].Count; j++) {
                        var kindChild = kindList[i][j];
                        analyzer.InvokeAnalysis(new AnalysisContextPrecusor {
                            Source = kindChild,
                            ContextProvider = contextProvider,
                            NextAction = prevNextAction,
                            CancellationToken = providedCt,
                        });
                    }
                }
                prevNextAction = NextAction;
            }
            prevNextAction(ct);
        }
    }

    internal class SyntaxAnalyzerGroupNode<TSourceAnalyzer, TSyntax> : AnalyzerGroupNode<SyntaxNode>, ISyntaxAnalyzerSetupGroup<TSyntax>
        where TSourceAnalyzer : SyntaxNodeAnalyzer<TSyntax>
        where TSyntax : SyntaxNode {

        public override Type AnalyzerType { get; } = typeof(TSourceAnalyzer);

        public SyntaxAnalyzerGroupNode(AnalyzerGroup<SyntaxNode> group)
            : base(group) {
        }

        private SyntaxAnalyzerGroupNode(AnalyzerGroupNode<SyntaxNode> node)
            : base(node.Group) {
        }

        private SyntaxAnalyzerGroupNode(AnalyzerGroupNode<SyntaxNode> node, Func<SyntaxNode, SyntaxNode> syntaxFactory)
            : base(node.Group, syntaxFactory) {
        }

        private SyntaxAnalyzerGroupNode(AnalyzerGroupNode<SyntaxNode> node, Func<SyntaxNode, IEnumerable<SyntaxNode>>? transitionMultiple)
            : base(node.Group, transitionMultiple) {
        }

        public ISyntaxAnalyzerSetupGroup<TSyntax> ThenUseAnalyzer<TAnalyzer>() where TAnalyzer : SyntaxNodeAnalyzer<TSyntax> {
            var nextItem = new SyntaxAnalyzerGroupNode<TAnalyzer, TSyntax>(this);
            SetNextNode(nextItem);
            return nextItem;
        }

        public ISyntaxAnalyzerSetupGroup<TNewSyntax> ThenUseAnalyzer<TAnalyzer, TNewSyntax>(Func<TSyntax, TNewSyntax> syntaxFactory) where TAnalyzer : SyntaxNodeAnalyzer<TNewSyntax> where TNewSyntax : SyntaxNode {
            var nextItem = new SyntaxAnalyzerGroupNode<TAnalyzer, TNewSyntax>(this, syntax => syntaxFactory((TSyntax)syntax));
            SetNextNode(nextItem);
            return nextItem;
        }

        public ISyntaxAnalyzerSetupGroup<TNewSyntax> ThenUseAnalyzer<TAnalyzer, TNewSyntax>(Func<TSyntax, IEnumerable<TNewSyntax>> syntaxFactory) where TAnalyzer : SyntaxNodeAnalyzer<TNewSyntax> where TNewSyntax : SyntaxNode {
            var nextItem = new SyntaxAnalyzerGroupNode<TAnalyzer, TNewSyntax>(this, syntax => syntaxFactory((TSyntax)syntax));
            SetNextNode(nextItem);
            return nextItem;
        }
    }

    internal class SyntaxAnalyzerSetup : ISyntaxAnalyzerSetup {

        private readonly List<AnalyzerGroup<SyntaxNode>> _anaylzerGroups = new List<AnalyzerGroup<SyntaxNode>>();

        public IReadOnlyList<AnalyzerGroup<SyntaxNode>> AnalyzerGroups => _anaylzerGroups;

        public ISyntaxAnalyzerSetupGroup<TSyntax> UseAnalyzer<TAnalyzer, TSyntax>()
            where TAnalyzer : SyntaxNodeAnalyzer<TSyntax>
            where TSyntax : SyntaxNode {
            var group = new AnalyzerGroup<SyntaxNode>();
            var first = new SyntaxAnalyzerGroupNode<TAnalyzer, TSyntax>(group);
            _anaylzerGroups.Add(group);
            return first;
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
            startup.ConfigureServices(_services);
            var syntaxActionSetup = new SyntaxAnalyzerSetup(_services);
            var symbolActionSetup = new SymbolAnalyzerSetup(_services);
            startup.ConfigureAnalyzer(new AnalyzerSetupContext(syntaxActionSetup, symbolActionSetup));

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
            foreach (var symbolAction in symbolActionGroups) {
                context.RegisterSymbolAction(symbolContext => { 
                    
                }, symbolAction.SymbolKinds);
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
