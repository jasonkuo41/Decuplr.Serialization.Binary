using System.Collections.Generic;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface ITypeParserDirector {
        void ComposeParser(IEnumerable<ITypeParserBuilder> layouts);
    }
}
