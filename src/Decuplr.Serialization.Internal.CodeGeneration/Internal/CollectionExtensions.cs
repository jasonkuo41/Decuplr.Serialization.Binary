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

#if NETSTANDARD2_0
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value) {
            key = pair.Key;
            value = pair.Value;
        }
#endif
    }
}
