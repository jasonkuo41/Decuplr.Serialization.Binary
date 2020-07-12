using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal sealed class ResultGenerator : IDisposable {

        private readonly IServiceScope _scope;
        private readonly TypeLayout _layout;
        private readonly IGenerationSource _provider;

        public ResultGenerator(TypeLayout layout, IServiceScope scope, IGenerationSource provider) {
            _layout = layout;
            _scope = scope;
            _provider = provider;
        }

        public IEnumerable<GeneratedSourceCode> GenerateFiles() {
            var provider = _scope.ServiceProvider;

        }

        public void Dispose() => _scope.Dispose();
    }
}
