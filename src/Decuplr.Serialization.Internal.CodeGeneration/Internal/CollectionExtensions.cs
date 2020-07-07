using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal static class CollectionExtensions {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> addFactory) {
            if (dict.TryGetValue(key, out var value))
                return value;
            value = addFactory(key);
            dict.Add(key, value);
            return value;
        }
    }
}
