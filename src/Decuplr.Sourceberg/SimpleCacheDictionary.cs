using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
    /// <summary>
    /// Represents a simple cache dictionary
    /// </summary>
    internal class SimpleCacheDictionary<TKey, TValue> {

        private readonly int _dropCount;
        private readonly LinkedList<TKey> _cacheQueue = new LinkedList<TKey>();
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TKey, LinkedListNode<TKey>> _cacheNodes = new Dictionary<TKey, LinkedListNode<TKey>>();

        public SimpleCacheDictionary(int dropCount) {
            if (dropCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(dropCount), dropCount, "Drop count cannot be less then zero");
            _dropCount = dropCount;
        }

        private void AddCacheKey(TKey key) {
            var node = _cacheQueue.AddLast(key);
            _cacheNodes.Add(key, node);
            if (_cacheQueue.Count > _dropCount) {
                // Remove least used cache
                var first = _cacheQueue.First;
                _cacheNodes.Remove(first.Value);
                _cache.Remove(first.Value);
                _cacheQueue.RemoveFirst();
            }
        }

        private void MarkCacheReused(TKey key) {
            var node = _cacheNodes[key];
            _cacheQueue.Remove(_cacheNodes[key]);
            _cacheQueue.AddLast(node);
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> addFactory) {
            if (_cache.TryGetValue(key, out var value)) {
                MarkCacheReused(key);
                return value;
            }
            value = addFactory(key);
            _cache.Add(key, value);
            AddCacheKey(key);
            return value;
        }

    }
}
