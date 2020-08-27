using System;
using System.Linq.Expressions;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public interface IThrowCollection {
        string AddException(Expression<Func<Exception>> expression);
    }
}