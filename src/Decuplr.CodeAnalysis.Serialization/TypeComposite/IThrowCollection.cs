﻿using System;
using System.Linq.Expressions;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    public interface IThrowCollection {
        string AddException(Expression<Func<Exception>> expression);
    }
}