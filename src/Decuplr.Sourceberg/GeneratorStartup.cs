using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Sourceberg.Generation {
    public abstract class GeneratorStartup {
        public abstract void ConfigureAnalyzer();
        public abstract void ConfigureServices(IServiceCollection services);
    }

    public abstract class SourceGenerator<TStartup> where TStartup : GeneratorStartup {

    }

    public interface ISyntaxAnalyzerSetup {
        ISyntaxAnalyzerSetup
    }

    public interface ISyntaxAnalyzerSetupGroup {
        ISyntaxAnalyzerSetupGroup ThenByAnalyzer<TAnalyzer>() where TAnalyzer : SyntaxNodeAnalyzer;
    }

    public interface ISymbolAnalyzerSetup {
        ISymbolAnalyzerSetupGroup UseAnalyzer<TAnalyzer>();
    }

    public interface ISymbolAnalyzerSetupGroup {
        ISymbolAnalyzerSetupGroup ThenByAnalyzer<TAnalyzer>();
    }

    public interface ICompilationDiagnosticReporter {

    }
}

namespace Decuplr.Sourceberg {

    public abstract class GeneratorAnalyzerBase {
        public abstract ImmutableArray<DiagnosticDescriptor> SupportedDescriptors { get; }
    }

    public abstract class SyntaxNodeAnalyzer : GeneratorAnalyzerBase {
        public abstract void RunAnalysis<TSyntax>(SyntaxContext<TSyntax> currentSyntax, DiagnosticResults context, CancellationToken ct) where TSyntax : SyntaxNode;
    }

    public abstract class SymbolActionAnalyzer : GeneratorAnalyzerBase {
        public abstract void RunAnalysis<TSymbol>(SymbolContext<TSymbol> currentSymbol, DiagnosticResults context, CancellationToken ct) where TSymbol : ISymbol;
    }

    public struct DiagnosticResults {

        private readonly Func<Diagnostic, bool> _supportedDiagnostic;
        private readonly Action<Diagnostic> _reportDiagnostic;

        public DiagnosticResults(Action<Diagnostic> reportDiagnostic, Func<Diagnostic, bool> isSupportedDiagnostic) {
            _reportDiagnostic = reportDiagnostic;
            _supportedDiagnostic = isSupportedDiagnostic;
        }

        public void ReportDiagnostic(Diagnostic diagnostic) {
            if (_supportedDiagnostic?.Invoke(diagnostic) ?? true)
                throw new ArgumentException($"Reported diagnostic with ID '{diagnostic.Id}' is not supported by the analyzer.", nameof(diagnostic));
            _reportDiagnostic?.Invoke(diagnostic);
        }
    }

    public class SourceFeatureContext {

        private readonly Dictionary<Type, object> _features = new Dictionary<Type, object>();


        [return: MaybeNull]
        public TFeature Get<TFeature>() {
            if (!_features.TryGetValue(typeof(TFeature), out var feature))
                return default;
            return (TFeature)feature;
        }

        public TFeature GetRequired<TFeature>() {
            if (!_features.TryGetValue(typeof(TFeature), out var feature))
                throw new NotSupportedException($"Context does not contain any {typeof(TFeature)} instances.");
            return (TFeature)feature;
        }

        public void Set<TFeature>(TFeature feature) {
            if (feature is null) {
                _features.Remove(typeof(TFeature));
                return;
            }
            _features[typeof(TFeature)] = feature;
        }

        public void SetUnique<TFeature>(TFeature feature) where TFeature : notnull {
            if (_features.ContainsKey(typeof(TFeature)))
                throw new NotSupportedException($"Context has contained more then one instances of {typeof(TFeature)}.");
            _features[typeof(TFeature)] = feature;
        }

        public IEnumerable<KeyValuePair<Type, object>> Features => _features;
    }

    public class SymbolContext<TSymbol> : SourceFeatureContext where TSymbol : ISymbol {
        public TSymbol Symbol { get; }

        internal SymbolContext(TSymbol currentSymbol) {
            Symbol = currentSymbol;
        }
    }

    public class SyntaxContext<TSyntax> : SourceFeatureContext where TSyntax : SyntaxNode {
        public TSyntax Syntax { get; }
    }
}
