using System;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis {
    public readonly struct IdentifierString : IEquatable<IdentifierString> {

        public string Name { get; }

        public IdentifierString(string name) {
            if (!SyntaxFacts.IsValidIdentifier(name))
                throw new ArgumentException("Invalid Generic Name", nameof(name));
            Name = name;
        }

        public override bool Equals(object obj) => obj is IdentifierString gName && Equals(gName);

        public bool Equals(IdentifierString other) => Name.Equals(other.Name);

        public override int GetHashCode() => Name.GetHashCode();

        public override string ToString() => Name;

        public static implicit operator IdentifierString(string name) => new IdentifierString(name);
    }
}
