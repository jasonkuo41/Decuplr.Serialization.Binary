using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.AnalysisService {
    class ReturnTypeMetaInfo : BaseTypeMetaInfo<ITypeSymbol> {

        public string Name => Symbol.Name;
        public bool IsVoid => Symbol.SpecialType == SpecialType.System_Void;

        public ReturnTypeMetaInfo(SourceCodeAnalysis analysis, ITypeSymbol symbol)
            : base (analysis, symbol) {
        }

        public override string ToString() => Symbol.ToString();

    }
}
