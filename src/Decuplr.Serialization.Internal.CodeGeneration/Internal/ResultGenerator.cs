using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal sealed class ResultGenerator : GeneratedResult {
        
        public ResultGenerator(IServiceScope scope, IEnumerable<Diagnostic> diagnostics, IGenerationSource provider, TypeLayout layout)
            : base (scope, diagnostics) {

        }


        protected override IEnumerable<string> GenerateFilesInternal() {

        }
    }
}
