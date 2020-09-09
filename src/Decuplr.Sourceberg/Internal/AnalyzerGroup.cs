using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Decuplr.Sourceberg.Services;
using Decuplr.Sourceberg.Services.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Decuplr.Sourceberg.Internal {
    internal class AnalyzerGroup<TKind> where TKind : notnull {

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

        public IServiceCollection RegisterServices(IServiceCollection services) {
#if !NETSTANDARD2_0
            var uniqueAnalyzers = new HashSet<Type>(_analyzers.Count);
#else
            var uniqueAnalyzers = new HashSet<Type>();
#endif
            foreach (var analyzer in _analyzers) {
                if (!uniqueAnalyzers.Add(analyzer.AnalyzerType))
                    continue;
                services.AddScoped(analyzer.AnalyzerType);
                services.AddScoped(typeof(SourceAnalyzerBase), service => service.GetRequiredService(analyzer.AnalyzerType));
            }
            return services;
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

}
