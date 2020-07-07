using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal abstract class GeneratedResult : IDisposable {

        private readonly IServiceScope _scope;
        private bool _isDisposed = false;

        protected IServiceProvider Services {
            get {
                ThrowIfDisposed();
                return _scope.ServiceProvider;
            }
        }

        public IEnumerable<Diagnostic> Diagnostics { get; }

        public bool IsFaulted { get; }

        protected GeneratedResult(IServiceScope scope, IEnumerable<Diagnostic> diagnostics) {
            Diagnostics = diagnostics;
            _scope = scope;
            IsFaulted = Diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error);
        }

        private void ThrowIfDisposed() {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(GeneratedResult));
        }

        protected abstract IEnumerable<string> GenerateFilesInternal();

        public virtual IEnumerable<string> GenerateFiles() {
            if (IsFaulted)
                throw new InvalidOperationException("Faulted result cannot produce correct files");
            return GenerateFilesInternal();
        }

        public void Dispose() {
            if (_isDisposed)
                return;
            _isDisposed = true;
            _scope.Dispose();
        }
    }
}
