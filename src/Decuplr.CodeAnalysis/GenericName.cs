using System;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis {
    public readonly struct GenericName : IEquatable<GenericName> {

        private readonly string _name;

        public GenericName(string name) {
            if (!SyntaxFacts.IsValidIdentifier(name))
                throw new ArgumentException("Invalid Generic Name", nameof(name));
            _name = name;
        }

        public override bool Equals(object obj) => obj is GenericName gName && Equals(gName);

        public bool Equals(GenericName other) => _name.Equals(other._name);

        public override int GetHashCode() => _name.GetHashCode();

        public static implicit operator GenericName(string name) => new GenericName(name);
    }
}
