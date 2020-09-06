using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Decuplr.Sourceberg.Internal {
    internal class SourceFeatureCollection : ISourceFeatureCollection {

        private readonly Dictionary<Type, object> _features = new Dictionary<Type, object>();

        public object? this[Type type] { 
            get {
                type.ThrowIfNull(nameof(type));
                if (!_features.TryGetValue(type, out var feature))
                    return default;
                return feature;
            }
            set {
                type.ThrowIfNull(nameof(type));
                if (value is null) {
                    _features.Remove(type);
                    return;
                }
                _features[type] = value;
            } 
        }

        [return: MaybeNull]
        public TFeature Get<TFeature>() {
            if (!_features.TryGetValue(typeof(TFeature), out var feature))
                return default;
            return (TFeature)feature;
        }

        public TFeature GetRequired<TFeature>() {
            if (!_features.TryGetValue(typeof(TFeature), out var feature))
                throw new NotSupportedException($"Context does not contain any {typeof(TFeature)} instances.");
            return (TFeature)feature;
        }

        public void Set<TFeature>(TFeature feature) {
            if (feature is null) {
                _features.Remove(typeof(TFeature));
                return;
            }
            _features[typeof(TFeature)] = feature;
        }

        public void SetUnique<TFeature>(TFeature feature) where TFeature : notnull {
            if (feature is null)
                throw new ArgumentNullException(nameof(feature));
            if (_features.ContainsKey(typeof(TFeature)))
                throw new NotSupportedException($"Context has contained more then one instances of {typeof(TFeature)}.");
            _features[typeof(TFeature)] = feature;
        }

        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator() => _features.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _features.GetEnumerator();

    }
}
