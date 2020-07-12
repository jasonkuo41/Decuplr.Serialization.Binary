using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal class ServiceScopeCollection : IDisposable {

        private readonly List<IServiceScope> _scopes = new List<IServiceScope>();

        public ServiceScopeCollection() { }

        public ServiceScopeCollection(int capacity) {
            _scopes = new List<IServiceScope>(capacity);
        }

        public void Add(IServiceScope scope) {
            _scopes.Add(scope);
        }

        public void Dispose() {
            foreach (var scope in _scopes) {
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
