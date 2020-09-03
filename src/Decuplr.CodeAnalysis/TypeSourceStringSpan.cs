using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis {
    /// <summary>
    /// Represents a segment of the <see cref="TypeSourceString"/>
    /// </summary>
    public readonly struct TypeSourceStringSpan : IEquatable<TypeSourceStringSpan> {

        /// <summary>
        /// The source of this span
        /// </summary>
        public TypeSourceString Source { get; }

        /// <summary>
        /// The starting index from the source
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// The length of the span
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// The name of this span
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The generics containing within this segment
        /// </summary>
        public IReadOnlyList<TypeSourceString> Generics { get; }

        public bool IsGeneric => Generics.Count != 0;

        internal TypeSourceStringSpan(TypeSourceString source, int start, int length) {
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

            IReadOnlyList<TypeSourceString> GetTypeString(ReadOnlySpan<char> str) {
                if (startBracket < 0)
                    return Array.Empty<TypeSourceString>();
                var endBracket = str.Length - 1;

                if (startBracket + 1 == endBracket)
                    throw InvalidType("Empty generic");
                Debug.Assert(str.LastIndexOf('>') == str.Length - 1);

                var genericsPart = str.Slice(startBracket + 1, endBracket).ToString();
                return genericsPart.Split(',').Select(x => TypeSourceString.FromCacheName(x)).ToList();
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

        public bool Equals(TypeSourceStringSpan other) => Source.Equals(other.Source) && Start.Equals(Start) && Length.Equals(Length);
        
        public override bool Equals(object obj) => obj is TypeSourceStringSpan span && Equals(span);
        public override int GetHashCode() => HashCode.Combine(Source, Start, Length);
        public override string ToString() {
            if (Generics.Count == 0)
                return Name;
            return $"{Name}<{string.Join(", ", Generics.Select(generic => generic.ToString()))}>";
        }
    }
}
