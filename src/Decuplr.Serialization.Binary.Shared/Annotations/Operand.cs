using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Decuplr.Serialization.Binary.Annotations {
    public enum Operand {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThenOrEqual,
        IsTypeOf,
        IsNotTypeOf,
    }
}
