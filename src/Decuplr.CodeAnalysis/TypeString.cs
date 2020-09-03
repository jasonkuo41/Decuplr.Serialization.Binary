using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis {

    public readonly struct TypeStringSpan : IEquatable<TypeStringSpan> {

        public TypeString Source { get; }

        public int Start { get; }

        public int Length { get; }

        public string Name { get; }

        public IReadOnlyList<TypeString> Generics { get; }

        internal TypeStringSpan(TypeString source, int start, int length) {
            Source = source;
            Start = start;
            Length = length;

            // Start validation
            // Type must end with brackets (if any)
            var str = source.FullName.AsSpan(Start, Length);
            var startBracket = str.IndexOf('<');

            EnsureEndsWithRightBracketIfAny(str);
            EnsureAngleBracketsInPair(str);

            Name = startBracket switch
            {
                0 => throw InvalidType("Cannot start with bracket"),
                _ when startBracket < 0 => str.ToString(),
                _ => str.Slice(0, startBracket - 1).ToString()
            };

            if (SyntaxFacts.IsValidIdentifier(Name))
                throw InvalidIdent();

            Generics = GetTypeString(str);

            Exception InvalidType(string reason) => throw new ArgumentException($"Invalid type name '{source.FullName}' ({reason})");
            Exception InvalidIdent() => InvalidType("Invalid identifier");

            IReadOnlyList<TypeString> GetTypeString(ReadOnlySpan<char> str) {
                if (startBracket < 0)
                    return Array.Empty<TypeString>();
                var endBracket = str.Length - 1;

                if (startBracket + 1 == endBracket)
                    throw InvalidType("Empty generic");
                Debug.Assert(str.LastIndexOf('>') == str.Length - 1);

                var genericsPart = str.Slice(startBracket + 1, endBracket).ToString();
                return genericsPart.Split(',').Select(x => TypeString.FromCacheName(x)).ToList();
            }

            void EnsureEndsWithRightBracketIfAny(ReadOnlySpan<char> str) {
                var lastRightBracket = str.LastIndexOf('>');
                if (lastRightBracket != -1 && lastRightBracket != str.Length - 1) {
                    throw InvalidType("'>' must be the final character)");
                }
            }

            void EnsureAngleBracketsInPair(ReadOnlySpan<char> str) {
                var leftAngleBrackCount = 0;
                for (var i = 0; i < str.Length; ++i) {
                    switch (str[i]) {
                        case '>':
                            leftAngleBrackCount--;
                            if (leftAngleBrackCount < 0)
                                throw InvalidType("Too much right angle brackets");
                            continue;
                        case '<':
                            leftAngleBrackCount++;
                            break;
                    }
                }
                if (leftAngleBrackCount != 0)
                    throw InvalidType("Too much left angle brackets");
            }
        }

        public bool Equals(TypeStringSpan other) => Source.Equals(other.Source) && Start.Equals(Start) && Length.Equals(Length);
        // override equal
    }

    /// <summary>
    /// Represents a light weight name counterpart of <see cref="TypeName"/> with lax comparsion rules.
    /// This class is meant for as a ease to use identifier and shall not be used to respresent any unique names of a type.
    /// </summary>
    public readonly struct TypeString : IEquatable<TypeString>, IEnumerable<TypeStringSpan>, IReadOnlyList<TypeStringSpan> {

        private static readonly ConcurrentDictionary<string, TypeString> _cache = new ConcurrentDictionary<string, TypeString>();

        private readonly IReadOnlyList<TypeStringSpan> _segments;

        public string FullName { get; }

        public TypeStringSpan this[int index] => _segments[index];

        public int SegmentCount => _segments.Count;

        internal TypeString(string typeName) {
            FullName = typeName;
            var segments = new List<TypeStringSpan>(8);
            _segments = segments;
            WriteSegments(this, segments, FullName);

            static void WriteSegments(TypeString str, List<TypeStringSpan> segments, string fullname) {
                var nameSpan = fullname.AsSpan();
                var currentIndex = 0;
                while (true) {
                    var seperatorIndex = nameSpan.IndexOf('.');
                    if (seperatorIndex < 0) {
                        if (fullname.Length > currentIndex)
                            segments.Add(new TypeStringSpan(str, currentIndex, fullname.Length - currentIndex));
                        return;
                    }
                    segments.Add(new TypeStringSpan(str, currentIndex, seperatorIndex - currentIndex));
                    nameSpan = nameSpan.Slice(seperatorIndex + 1);
                    currentIndex += seperatorIndex + 1;
                }
            }
        }

        private static string Concentrate(string source, ReadOnlySpan<char> seperators) {
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

        internal static TypeString FromCacheName(string cacheName) {
            return _cache.GetOrAdd(cacheName, Create);

            static TypeString Create(string name) => new TypeString(name);
        }

        public static TypeString FromCodeName(string typeName) => FromCacheName(Concentrate(typeName, stackalloc[] { ',', '.', '<', '>' }));

        public IEnumerator<TypeStringSpan> GetEnumerator() => _segments.GetEnumerator();

        int IReadOnlyCollection<TypeStringSpan>.Count => SegmentCount;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(TypeString other) => FullName.Equals(other.FullName);
        public override bool Equals(object obj) => (obj is TypeString typeString && Equals(typeString)) || (obj is string str && str == FullName);
        public override int GetHashCode() => FullName.GetHashCode();
    }
}
