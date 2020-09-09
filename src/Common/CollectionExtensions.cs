using System;
using System.Collections.Generic;
using System.Linq;

namespace Decuplr {
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
            foreach (var removingKey in removingKeys) {
                removeCount += dict.Remove(removingKey) ? 1 : 0;
            }
            return removeCount;
        }

#if NETSTANDARD2_0
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value) {
            key = pair.Key;
            value = pair.Value;
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N) {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> values, T value) {
            yield return value;
            foreach (T item in values) {
                yield return item;
            }
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> values, T value) {
            foreach (T item in values) {
                yield return item;
            }
            yield return value;
        }
#endif
    }
}
