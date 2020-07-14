using System;
using System.Linq.Expressions;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IThrowCollection {
        string AddException(Expression<Func<Exception>> expression);
    }
}