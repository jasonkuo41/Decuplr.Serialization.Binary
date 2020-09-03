using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {

    /// <summary>
    /// Represents a type that is expressed and can be used in source. Provides easy to identification capability.
    /// </summary>
    public readonly struct TypeSourceString : IEquatable<TypeSourceString>, IEnumerable<TypeSourceStringSpan>, IReadOnlyList<TypeSourceStringSpan> {

        private static readonly ConcurrentDictionary<Type, TypeSourceString> _typeCache = new ConcurrentDictionary<Type, TypeSourceString>();
        private static readonly ConcurrentDictionary<string, TypeSourceString> _nameCache = new ConcurrentDictionary<string, TypeSourceString>();

        private readonly IReadOnlyList<TypeSourceStringSpan> _spans;

        /// <summary>
        /// The full name (the full source representation).
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// The last segment, normally is what the type is
        /// </summary>
        public TypeSourceStringSpan LastSpan => _spans[_spans.Count - 1];

        public TypeSourceStringSpan this[int index] => _spans[index];

        public int SpanCount => _spans.Count;

        internal TypeSourceString(string typeName) {
            FullName = typeName;
            var segments = new List<TypeSourceStringSpan>(8);
            _spans = segments;
            WriteSegments(this, segments, FullName);

            static void WriteSegments(TypeSourceString str, List<TypeSourceStringSpan> segments, string fullname) {
                var nameSpan = fullname.AsSpan();
                var currentIndex = 0;
                while (true) {
                    var seperatorIndex = nameSpan.IndexOf('.');
                    if (seperatorIndex < 0) {
                        if (fullname.Length > currentIndex)
                            segments.Add(new TypeSourceStringSpan(str, currentIndex, fullname.Length - currentIndex));
                        return;
                    }
                    segments.Add(new TypeSourceStringSpan(str, currentIndex, seperatorIndex - currentIndex));
                    nameSpan = nameSpan.Slice(seperatorIndex + 1);
                    currentIndex += seperatorIndex + 1;
                }
            }
        }

        private static string ConcentrateCodeName(string source, ReadOnlySpan<char> seperators) {
            var writer = new SpanWriter<char>(stackalloc char[source.Length]);
            var currentSpan = source.AsSpan();
            while (true) {
                var currentIndex = currentSpan.IndexOfAny(seperators);
                if (currentIndex < 0) {
                    writer.Write(currentSpan.Trim());
                    return writer.Current.ToString();
                }
                writer.Write(currentSpan.Slice(0, currentIndex).Trim());
                writer.Write(currentSpan[currentIndex]);
                currentSpan = currentSpan.Slice(currentIndex + 1);
            }
        }

        private static TypeSourceString TypeToSourceString(Type type) {
            var builder = new StringBuilder();
            builder.Append(type.Namespace);
            builder.Append('.');
            foreach (var parent in type.GetAllDeclaringTypes()) {
                builder.Append(GetSourceString(parent));
                builder.Append('.');
            }
            builder.Append(GetSourceString(type));

            return FromCacheName(builder.ToString());

            static string GetSourceString(Type type) {
                var genericPos = type.Name.IndexOf('`');
                var builder = new StringBuilder();
                if (genericPos < 0) {
                    builder.Append(type.Name);
                    return builder.ToString();
                }
                // remove List`1 's '`1'
                builder.Append(type.Name.Substring(0, genericPos));
                builder.Append('<');
                builder.Append(string.Join(",", type.GetGenericArguments().Select(typeArg => GetSourceString(typeArg))));
                builder.Append('>');
                return builder.ToString();
            }
        }

        internal static TypeSourceString FromCacheName(string cacheName) => _nameCache.GetOrAdd(cacheName, name => new TypeSourceString(name));

        public static TypeSourceString FromName(string typeName) => FromCacheName(ConcentrateCodeName(typeName, stackalloc[] { ',', '.', '<', '>' }));

        public static TypeSourceString FromType(Type type) => _typeCache.GetOrAdd(type, TypeToSourceString);

        public static TypeSourceString FromSymbol(INamedTypeSymbol typeSymbol) => FromCacheName(typeSymbol.ToString());

        public IEnumerator<TypeSourceStringSpan> GetEnumerator() => _spans.GetEnumerator();

        int IReadOnlyCollection<TypeSourceStringSpan>.Count => SpanCount;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator TypeSourceString(string str) => FromName(str);
        public static implicit operator TypeSourceString(Type type) => FromType(type);
        public static implicit operator TypeSourceString(TypeName typeName) => typeName.SourceString;

        public bool Equals(TypeSourceString other) => FullName.Equals(other.FullName);
        public override bool Equals(object obj) => (obj is TypeSourceString typeString && Equals(typeString)) || (obj is string str && str == FullName);
        public override int GetHashCode() => FullName.GetHashCode();
    }
}
