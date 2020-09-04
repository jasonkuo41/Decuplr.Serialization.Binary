using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decuplr.Sourceberg {
    internal static class CollectionExtensions {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> addFactory) {
            if (dict.TryGetValue(key, out var value))
                return value;
            value = addFactory(key);
            dict.Add(key, value);
            return value;
        }

        public static int RemoveWhere<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> predicate) {
            var removingKeys = dict.Where(predicate).Select(x => x.Key);
            var removeCount = 0;
            foreach(var removingKey in removingKeys) {
                removeCount += dict.Remove(removingKey) ? 1 : 0;
            }
            return removeCount;
        }

#if NETSTANDARD2_0
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value) {
            key = pair.Key;
            value = pair.Value;
        }
#endif
    }
}
