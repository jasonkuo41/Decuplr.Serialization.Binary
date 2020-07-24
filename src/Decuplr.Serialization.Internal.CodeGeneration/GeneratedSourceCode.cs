using System;

namespace Decuplr.Serialization.CodeGeneration {

    [Obsolete]
    public struct GeneratedSourceCode {

        public GeneratedSourceCode(string fileName, string sourceText) {
            FileName = fileName;
            SourceText = sourceText;
        }

        public string FileName { get; set; }
        public string SourceText { get; set; }

        public static implicit operator GeneratedSourceCode((string, string) tuple) => new GeneratedSourceCode(tuple.Item1, tuple.Item2);
    }
}
