using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal static class GeneratedResultExtensions {
        private class GeneratedResultCollection : ISourceGeneratedResults {

            private readonly List<GeneratedResult> _results;

            public bool IsFaulted => _results.Any(x => x.IsFaulted);

            public IEnumerable<Diagnostic> Diagnostics => _results.SelectMany(x => x.Diagnostics);

            public GeneratedResultCollection(IEnumerable<GeneratedResult> results) {
                _results = results.ToList();
            }

            public IEnumerable<string> GenerateFiles() => _results.SelectMany(x => x.GenerateFiles());

            public void Dispose() {
                foreach (var result in _results)
                    result.Dispose();
            }

        }

        public static ISourceGeneratedResults ToGeneratedResults(this IEnumerable<GeneratedResult?> results) => new GeneratedResultCollection(results.Where(x => x != null)!);
    }
}
