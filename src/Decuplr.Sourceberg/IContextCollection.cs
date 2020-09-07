using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Decuplr.Sourceberg {
    public interface IContextCollection : IEnumerable<KeyValuePair<Type, object>> {
        object? this[Type type] { get; set; }

        [return: MaybeNull]
        TContext Get<TContext>();
        TContext GetRequired<TContext>();
        void Set<TContext>(TContext feature);
        void SetUnique<TContext>(TContext feature) where TContext : notnull;
    }
}
