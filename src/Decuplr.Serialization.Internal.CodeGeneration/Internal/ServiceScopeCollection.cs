using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal class ServiceScopeCollection : IDisposable {

        private IServiceScope[] _scopes = new IServiceScope[4];
        private int _current;

        public ServiceScopeCollection() { }

        public ServiceScopeCollection(int capacity) {
            _scopes = new IServiceScope[capacity];
        }

        private void Allocate() {
            var replacer = new IServiceScope[_scopes.Length * 2];
            Array.Copy(_scopes, replacer, _scopes.Length);
            _scopes = replacer;
        }

        public ref IServiceScope CreateScopeBlock() {
            _current++;
            if (_current >= _scopes.Length)
                Allocate();
            return ref _scopes[_current];
        }

        public void Dispose() {
            foreach (var scope in _scopes) {
                if (scope is null)
                    continue;
                if (scope is IAsyncDisposable asyncDisposable) {
                    var valueTask = asyncDisposable.DisposeAsync();
                    if (!valueTask.IsCompleted)
                        valueTask.AsTask().GetAwaiter().GetResult();
                }
                scope.Dispose();
            }
        }
    }

}
