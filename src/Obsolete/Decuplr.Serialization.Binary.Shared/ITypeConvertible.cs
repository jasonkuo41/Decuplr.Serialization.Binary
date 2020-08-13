using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    public interface ITypeConvertible<T> {
        T ConvertTo();
    }
}
