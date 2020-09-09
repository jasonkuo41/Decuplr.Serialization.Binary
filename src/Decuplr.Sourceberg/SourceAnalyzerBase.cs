using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Decuplr.Sourceberg.Services;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {

    /// <summary>
    /// Represents the base class of the Sourceberg Analyzers. This class cannot be inherit directly.
    /// </summary>
    public abstract class SourceAnalyzerBase {

        private readonly static ConcurrentDictionary<Type, ImmutableHashSet<string>> _typeSupportedDiagnostics = new ConcurrentDictionary<Type, ImmutableHashSet<string>>();
        private readonly ImmutableHashSet<string> _supportedDiagnostics;
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        internal IReadOnlyList<Diagnostic> ReportingDiagnostics => _diagnostics;

        // Protecteds this class to be inherit directly
        private protected SourceAnalyzerBase() {
            _supportedDiagnostics = GetSupportedDiagnostics(GetType());
        }

        internal static ImmutableHashSet<string> GetSupportedDiagnostics(Type type) {
            if (type.IsSubclassOf(typeof(SourceAnalyzerBase)))
                throw new ArgumentException($"Invalid Type, Type '{type}' is not derived from '{nameof(SourceAnalyzerBase)}'");
            return _typeSupportedDiagnostics.GetOrAdd(type, GetSupportFromAttribute);

            static ImmutableHashSet<string> GetSupportFromAttribute(Type type) 
                => type.GetCustomAttributes(type, true)
                       .Cast<UseDiagnosticsAttribute>()
                       .SelectMany(x => x.SupportedDiagnostics)
                       .ToImmutableHashSet();
        }

        /// <summary>
        /// Reports a diagnostic.
        /// </summary>
        /// <param name="diagnostic">The diagnostic to be reported</param>
        protected void ReportDiagnostic(Diagnostic diagnostic) {
            if (!_supportedDiagnostics.Contains(diagnostic.Id))
                throw new InvalidOperationException($"Reporting a non supported diagnostic (Id : {diagnostic.Id} - '{diagnostic}') is invalid");
            _diagnostics.Add(diagnostic);
        }

        internal abstract void InvokeAnalysis<TContext>(TContext context, Action<CancellationToken> nextAction);
    }
}
