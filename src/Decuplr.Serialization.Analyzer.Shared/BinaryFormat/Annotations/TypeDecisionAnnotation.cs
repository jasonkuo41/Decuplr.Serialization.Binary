using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    // Decides how the member should be formatted
    public abstract class TypeDecisionAnnotation {
        public abstract IReadOnlyList<INamedTypeSymbol> RequestParserType { get; }
    }
}
