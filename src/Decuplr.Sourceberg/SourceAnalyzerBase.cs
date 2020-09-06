using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {

    /// <summary>
    /// Represents the base class of the Sourceberg Analyzers. This class cannot be inherit directly.
    /// </summary>
    public abstract class SourceAnalyzerBase {

        // Protecteds this class to be inherit directly
        private protected SourceAnalyzerBase() { }

        /// <summary>
        /// The diagnostic descriptors this analyzer would produce
        /// </summary>
        public abstract ImmutableArray<DiagnosticDescriptor> SupportedDescriptors { get; }
    }
}
