using System;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis {
    public readonly struct GenericName : IEquatable<GenericName> {

        public string Name { get; }

        public GenericName(string name) {
            if (!SyntaxFacts.IsValidIdentifier(name))
                throw new ArgumentException("Invalid Generic Name", nameof(name));
            Name = name;
        }

        public override bool Equals(object obj) => obj is GenericName gName && Equals(gName);

        public bool Equals(GenericName other) => Name.Equals(other.Name);

        public override int GetHashCode() => Name.GetHashCode();

        public override string ToString() => Name;

        public static implicit operator GenericName(string name) => new GenericName(name);
    }
}
