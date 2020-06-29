using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Decuplr.Serialization.Binary.Annotations {
    public enum Operator {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        IsTypeOf,
        IsNotTypeOf,
    }
}
