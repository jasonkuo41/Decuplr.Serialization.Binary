using System.Collections.Generic;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal struct EmbeddedCode {
        public IReadOnlyList<string> CodeNamespaces { get; set; }
        public string SourceCode { get; set; }

        public bool IsEmpty => string.IsNullOrEmpty(SourceCode);
    }
}
