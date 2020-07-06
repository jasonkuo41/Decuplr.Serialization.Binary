using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface ISourceGeneratedResult {
        IEnumerable<Diagnostic> Diagnostics { get; }

        string GenerateFiles();
    }
}
