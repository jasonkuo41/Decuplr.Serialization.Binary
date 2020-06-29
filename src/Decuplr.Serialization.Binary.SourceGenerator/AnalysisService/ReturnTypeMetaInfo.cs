using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.AnalysisService {
    class ReturnTypeMetaInfo {

        private readonly SourceCodeAnalysis Analysis;

        public ITypeSymbol Symbol { get; }

    }
}
