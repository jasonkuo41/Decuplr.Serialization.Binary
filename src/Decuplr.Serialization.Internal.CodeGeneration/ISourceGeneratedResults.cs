using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface ISourceGeneratedResults : IDisposable {

        bool IsFaulted { get; }

        IEnumerable<Diagnostic> Diagnostics { get; }

        IEnumerable<GeneratedSourceCode> GenerateFiles();
    }
}
