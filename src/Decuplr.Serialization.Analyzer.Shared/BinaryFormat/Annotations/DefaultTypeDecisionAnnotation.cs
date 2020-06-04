using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    public class DefaultTypeDecisionAnnotation : TypeDecisionAnnotation {
        public override IReadOnlyList<INamedTypeSymbol> RequestParserType { get; }

        public DefaultTypeDecisionAnnotation(INamedTypeSymbol defaultSymbol) {
            RequestParserType = new INamedTypeSymbol[] { defaultSymbol };
        }

    }
}
