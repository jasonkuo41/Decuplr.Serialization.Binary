using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    public class DefaultTypeDecisionAnnotation : TypeDecisionAnnotation {
        public override IReadOnlyList<ITypeSymbol> RequestParserType { get; }

        public DefaultTypeDecisionAnnotation(ITypeSymbol defaultSymbol) {
            RequestParserType = new ITypeSymbol[] { defaultSymbol };
        }

    }
}
