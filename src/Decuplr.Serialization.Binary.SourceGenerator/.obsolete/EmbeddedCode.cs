using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decuplr.Serialization.Binary {
    [Obsolete]
    internal struct EmbeddedCode {
        public IReadOnlyList<string> CodeNamespaces { get; set; }
        public string SourceCode { get; set; }

        public bool IsEmpty => string.IsNullOrEmpty(SourceCode);
        public EmbeddedCode Merge(EmbeddedCode code) {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(SourceCode);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(code.SourceCode);
            return new EmbeddedCode {
                CodeNamespaces = (CodeNamespaces ?? Enumerable.Empty<string>()).Concat(code.CodeNamespaces ?? Enumerable.Empty<string>()).ToList(),
                SourceCode = stringBuilder.ToString()
            };
        }
    }
}
