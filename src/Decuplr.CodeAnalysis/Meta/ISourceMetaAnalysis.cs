using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Meta {
    public interface ISourceMetaAnalysis {
        IEnumerable<NamedTypeMetaInfo> GetMetaInfo();
        IEnumerable<NamedTypeMetaInfo> GetMetaInfo(Func<ISymbol, bool> predicate);
    }

}
