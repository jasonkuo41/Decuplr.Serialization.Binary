using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis {

    public readonly struct TypeStringSpan : IEquatable<TypeStringSpan> {

        public TypeString Source { get; }

        public int Start { get; }

        public int Length { get; }

        public IEnumerable<TypeStringSpan> GetGenerics() {

        }

        public bool Equals(TypeStringSpan other) {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// Represents a light weight name counterpart of <see cref="TypeName"/> with lax comparsion rules.
    /// This class is meant for as a ease to use identifier and shall not be used to respresent any unique names of a type.
    /// </summary>
    public readonly struct TypeString : IEquatable<TypeString>, IEnumerable<TypeStringSpan>, IReadOnlyList<TypeStringSpan> {
        internal static string[] VerifyNamespaces(string fullTypeName, string argName) {
            var sliced = fullTypeName.Split('.');
            if (sliced.Any(x => !SyntaxFacts.IsValidIdentifier(x.Trim())))
                throw new ArgumentException($"Invalid type identification name : {fullTypeName}", argName);
            return sliced;
        }

        internal static string[] VerifyTypeNames(string fullTypeNames, string argName) {
            var slicedType = fullTypeNames.Split('.');
            if (slicedType.Any(x => TypeWithClampedString(x).Any(x => !SyntaxFacts.IsValidIdentifier(x))))
                throw ThrowArgException();
            return slicedType;

            IEnumerable<string> TypeWithClampedString(string source) {
                var startBracket = source.IndexOf('<');
                yield return startBracket < 0 ? source.Trim() : source.Substring(0, startBracket).Trim();
                foreach (var clamped in GetClampedStrings(source))
                    yield return clamped;
            }

            IEnumerable<string> GetClampedStrings(string source) {
                var startBracket = source.IndexOf('<');
                var endBracket = source.LastIndexOf('>');
                if (startBracket < 0 ^ endBracket < 0)
                    throw ThrowArgException();
                if (startBracket < 0 || endBracket < 0)
                    return Enumerable.Empty<string>();
                var clampedString = source.Substring(startBracket + 1, endBracket - startBracket - 1);
                return clampedString.Split(',').Select(x => x.Trim());
            }

            Exception ThrowArgException() => new ArgumentException($"Invalid type identification name : {fullTypeNames}", argName);
        }

        private readonly IReadOnlyList<TypeStringSpan> _segments;

        public string FullName { get; }

        public TypeStringSpan this[int index] => _segments[index];

        public int SegmentCount => _segments.Count;

        public TypeString(string typeName) {
            // we will concentrate the string for consumption
            var types = typeName.Split('.').Select(x => x.Trim()).ToArray();
            foreach(var type in types) {
                // check if > is the last element or not found, otherwise it's not a valid type string

            }

            var segments = new List<TypeStringSpan>(8);
            var typeSpan = typeName.AsSpan();
            var lastIndex = 0;
            var currentIndex = typeSpan.IndexOf('.');
            while (currentIndex > 0) {
                var segmentSpan = typeSpan.Slice(lastIndex, currentIndex);
                segments.Add(new TypeStringSpan(segmentSpan));
            }

            string Concentrate(char[] identifiers, string originalString) {
                var currentLength = 0;
                Span<char> str = stackalloc char[originalString.Length];
                var lastIndex = 0;
                var spaceStart = 0;
                var spaceLength = 0;
                for(var i = 0; i < originalString.Length; ++i) {
                    if (originalString[i] == ' ') {
                        if (spaceStart == -1)
                            spaceStart = i;
                        spaceLength++;
                    }
                    if (identifiers.Contains(originalString[i])) {
                        var length = spaceStart - lastIndex + 1;
                        originalString.AsSpan(lastIndex, length).CopyTo(str.Slice(currentLength));
                        currentLength += length;
                        spaceStart = -1;
                        lastIndex = i;
                    }
                    else {
                        spaceStart = -1;
                    }
                }
                return new string(str.Slice(0, currentLength));
            }
        }

        public IEnumerator<TypeStringSpan> GetEnumerator() => _segments.GetEnumerator();

        int IReadOnlyCollection<TypeStringSpan>.Count => SegmentCount;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
