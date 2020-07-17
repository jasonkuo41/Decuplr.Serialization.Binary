using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal class FaultedSourceGeneratedResults : ISourceGeneratedResults {

        private readonly ServiceScopeCollection _scopes;

        public bool IsFaulted => true;

        public IEnumerable<Diagnostic> Diagnostics { get; }

        public FaultedSourceGeneratedResults(IEnumerable<Diagnostic> diagnostics, ServiceScopeCollection scopes) {
            Diagnostics = diagnostics;
            _scopes = scopes;
        }

        public IEnumerable<GeneratedSourceCode> GenerateFiles() => throw new InvalidOperationException("Faulted results are uncapable of generating file results");

        public void Dispose() => _scopes.Dispose();

    }

}
