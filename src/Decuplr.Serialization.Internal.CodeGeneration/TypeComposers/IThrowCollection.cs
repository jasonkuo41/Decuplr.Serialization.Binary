using System;
using System.Linq.Expressions;

namespace Decuplr.Serialization.CodeGeneration.TypeComposers {
    public interface IThrowCollection {
        string AddException(Expression<Func<Exception>> expression);
    }
}