using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal sealed class ResultGenerator {

        private readonly TypeLayout _layout;
        private readonly IGenerationStartup _provider;

        public ResultGenerator(TypeLayout layout, IGenerationStartup provider) {
            _layout = layout;
            _provider = provider;
        }

        public IEnumerable<GeneratedSourceCode> GenerateFiles() {
            var provider = _scope.ServiceProvider;

        }

    }
}
