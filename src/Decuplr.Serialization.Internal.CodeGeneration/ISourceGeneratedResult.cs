using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface ISourceGeneratedResult {

        bool IsFaulted { get; }

        IEnumerable<Diagnostic> Diagnostics { get; }

        string GenerateFiles();
    }
}
