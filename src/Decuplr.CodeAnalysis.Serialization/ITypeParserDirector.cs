using System.Collections.Generic;
using System.Threading;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface ITypeParserDirector {
        void ComposeParser(IEnumerable<ITypeParserBuilder> layouts);
    }
}
