using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Decuplr.Sourceberg {
    public interface ISourceFeatureCollection : IEnumerable<KeyValuePair<Type, object>> {
        object? this[Type type] { get; set; }

        [return: MaybeNull]
        TFeature Get<TFeature>();
        TFeature GetRequired<TFeature>();
        void Set<TFeature>(TFeature feature);
        void SetUnique<TFeature>(TFeature feature) where TFeature : notnull;
    }
}
