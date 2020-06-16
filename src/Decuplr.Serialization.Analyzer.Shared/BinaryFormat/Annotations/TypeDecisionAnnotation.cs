using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    [Obsolete]
    // Decides how the member should be formatted
    public abstract class TypeDecisionAnnotation {
        public abstract IReadOnlyList<ITypeSymbol> RequestParserType { get; }
    }
}
