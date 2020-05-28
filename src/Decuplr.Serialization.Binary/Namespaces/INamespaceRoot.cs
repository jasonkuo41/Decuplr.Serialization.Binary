using System;
using System.Collections;
using System.Collections.Generic;

namespace Decuplr.Serialization.Binary.Namespaces {
    /// <summary>
    /// Represents a neutral, unmodified and root of parsers namespace
    /// </summary>
    public interface INamespaceRoot {
        IDefaultParserNamespace DefaultNamespace { get; }
        IParserNamespace GetNamespace(IEnumerable<string> parserNamespace);
    }

}
