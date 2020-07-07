using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal class FaultedGeneratedResult : GeneratedResult {
        public FaultedGeneratedResult(IServiceScope scope, IEnumerable<Diagnostic> diagnostics) 
            : base(scope, diagnostics) {
        }

        protected override IEnumerable<string> GenerateFilesInternal() => throw new InvalidOperationException("Faulted result cannot produce correct files");
    }
}
