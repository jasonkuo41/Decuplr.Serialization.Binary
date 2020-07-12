using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal static class GeneratedResultExtensions {
        private class GeneratedResultCollection : ISourceGeneratedResults {

            private readonly List<ResultGenerator> _results;

            public bool IsFaulted => false;

            public IEnumerable<Diagnostic> Diagnostics { get; }

            public GeneratedResultCollection(IEnumerable<ResultGenerator> results, IEnumerable<Diagnostic> diagnostics) {
                _results = results.ToList();
                Diagnostics = diagnostics;
            }

            public IEnumerable<GeneratedSourceCode> GenerateFiles() => _results.SelectMany(x => x.GenerateFiles());

            public void Dispose() {
                foreach (var result in _results)
                    result.Dispose();
            }

        }

        public static ISourceGeneratedResults ToGeneratedResults(this IEnumerable<ResultGenerator> results, IEnumerable<Diagnostic> diagnostics) => new GeneratedResultCollection(results.Where(x => x != null)!, diagnostics);
    }
}
