using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    public class BitUnionAnnotation : TypeDecisionAnnotation {

        public int UnionSize { get; }

        public int[][] UnionLayout { get; }

        public override IReadOnlyList<INamedTypeSymbol> RequestParserType => throw new System.NotImplementedException();
    }

}
