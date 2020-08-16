using System;
using System.Reflection;

namespace Decuplr.Serialization.Namespaces {
    /// <summary>
    /// The entry info for this type for namespace containers
    /// </summary>
    public readonly struct TypeEntryInfo : IEquatable<TypeEntryInfo> {
        /// <summary>
        /// The associated type
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The assembly responsible for adding the type
        /// </summary>
        public Assembly SourceAssembly { get; }

        public TypeEntryInfo(Assembly sourceAssembly, Type type) {
            SourceAssembly = sourceAssembly;
            Type = type;
        }

        /// <inheritdoc/>
        public bool Equals(TypeEntryInfo other) => Type.Equals(other.Type) && SourceAssembly.Equals(other.SourceAssembly);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is TypeEntryInfo other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Type, SourceAssembly);

        public static implicit operator TypeEntryInfo((Assembly, Type) type) => new TypeEntryInfo(type.Item1, type.Item2);

        public void Deconstruct(out Assembly assembly, out Type type) {
            assembly = SourceAssembly;
            type = Type;
        }
    }
}
