using System;
using System.Linq.Expressions;

namespace Decuplr.Serialization.CodeGeneration.TypeComposite {
    public interface IThrowCollection {
        string AddException(Expression<Func<Exception>> expression);
    }
}