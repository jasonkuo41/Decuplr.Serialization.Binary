using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    public interface IOverrideParserAttribute {
        string ParserNamespace { get; }
    }
}
