using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Decuplr.Sourceberg.Services;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {

    internal struct AnalysisContextPrecusor {
        public object Source { get; set; }
        public IContextCollectionProvider ContextProvider { get; set; }
        public Action<CancellationToken> NextAction { get; set; }
        public Action<Diagnostic> OnDiagnostics { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }

    /// <summary>
    /// Represents the base class of the Sourceberg Analyzers. This class cannot be inherit directly.
    /// </summary>
    public abstract class SourceAnalyzerBase {

        private ImmutableHashSet<DiagnosticDescriptor>? _descriptors;

        // Protecteds this class to be inherit directly
        private protected SourceAnalyzerBase() { }

        /// <summary>
        /// The diagnostic descriptors this analyzer would produce
        /// </summary>
        public abstract ImmutableArray<DiagnosticDescriptor> SupportedDescriptors { get; }


        protected bool IsSupportedDiagnostic(Diagnostic diagnostic) {
            _descriptors ??= SupportedDescriptors.ToImmutableHashSet();
            return _descriptors.Contains(diagnostic.Descriptor);
        }


        internal abstract void InvokeAnalysis(AnalysisContextPrecusor contextPrecusor);
    }
}
