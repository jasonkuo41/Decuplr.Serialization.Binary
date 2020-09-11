﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Decuplr.Sourceberg.Services;
using Decuplr.Sourceberg.Services.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Decuplr.Sourceberg.Internal {

    internal struct AnalyzerGroupResult {
        public IEnumerable<Diagnostic> GeneratedDiagnostics { get; set; }
        public IContextCollectionProvider 
    }

    internal class AnalyzerGroup<TKind, TContext> where TKind : notnull {

        private readonly List<AnalyzerGroupNode<TKind, TContext>> _analyzers = new List<AnalyzerGroupNode<TKind, TContext>>();
        public IReadOnlyList<AnalyzerGroupNode<TKind, TContext>> Analyzers => _analyzers;

        internal void Add(AnalyzerGroupNode<TKind, TContext> node) => _analyzers.Add(node);

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

        private IEnumerable<Diagnostic> InvokeScopeCore(IServiceProvider service,
                                                        TKind kind,
                                                        Compilation? compilation,
                                                        Func<TKind, bool, TContext> context,
                                                        IContextCollectionProvider? externalProvider,
                                                        CancellationToken ct) {
            using var scope = service.CreateScope();
            var scopeService = scope.ServiceProvider;
            var accessor = scopeService.GetRequiredService<SourceContextAccessor>();
            {
                accessor.OnOperationCanceled = ct;
                accessor.SourceCompilation = compilation;
            }
            var contextProvider = externalProvider ?? scopeService.GetRequiredService<IContextCollectionProvider>();

            var kindList = ExpandKindForInvocation(kind);

            // reverse iterator
            Action<CancellationToken> prevNextAction = ct => { };
            for (var i = Analyzers.Count - 1; i >= 0; --i) {
                var analyzerInfo = Analyzers[i];
                var analyzer = (SourceAnalyzerBase)scopeService.GetRequiredService(analyzerInfo.AnalyzerType);
                var currentIndex = i;

                void NextAction(CancellationToken providedCt) {
                    for (int j = 0; j < kindList[currentIndex].Count; j++) {
                        var kindChild = kindList[currentIndex][j];
                        analyzer.InvokeAnalysis(context(kindChild, currentIndex == 0), prevNextAction);
                    }
                }
                prevNextAction = NextAction;
            }
            prevNextAction(ct);

            return scopeService.GetServices<SourceAnalyzerBase>().SelectMany(x => x.ReportingDiagnostics);
        }

        public IEnumerable<Diagnostic> InvokeScope(IServiceProvider service,
                                                   TKind kind,
                                                   Compilation? compilation,
                                                   Func<TKind, bool, TContext> context,
                                                   IContextCollectionProvider externalProvider,
                                                   CancellationToken ct) 
            => InvokeScopeCore(service, kind, compilation, context, externalProvider, ct);

        public IEnumerable<Diagnostic> InvokeScope(IServiceProvider service,
                                                   TKind kind,
                                                   Compilation? compilation,
                                                   Func<TKind, bool, TContext> context,
                                                   CancellationToken ct) 
            => InvokeScopeCore(service, kind, compilation, context, null, ct);
    }

}
