using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

// TODO : Move to Decuplr.Serialization.Primitives assembly and move to namespace Decuplr.Serialization.Annotations
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
