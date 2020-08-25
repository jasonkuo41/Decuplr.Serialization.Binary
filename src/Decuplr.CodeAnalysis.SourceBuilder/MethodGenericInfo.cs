using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    public struct MethodGenericInfo : IEquatable<MethodGenericInfo> {
        public string GenericName { get; }
        public TypeKind? ConstrainedKind { get; }
        public IReadOnlyList<TypeName> ConstrainedTypes { get; }

        public bool HasConstrain => ConstrainedKind != null || ConstrainedTypes.Count > 0;

        private MethodGenericInfo(string genericName, TypeKind? constrainedKind, IEnumerable<TypeName>? constrainedTypes) {
            if (constrainedKind is { } && constrainedKind != TypeKind.Class && constrainedKind != TypeKind.Struct)
                throw new ArgumentOutOfRangeException(nameof(constrainedKind), constrainedKind, "Constrained Type can only be class or struct");
            genericName.EnsureValidIdentifier();
            GenericName = genericName;
            ConstrainedKind = constrainedKind;
            ConstrainedTypes = constrainedTypes?.ToArray() ?? Array.Empty<TypeName>();
        }

        public MethodGenericInfo(string genericName) : this(genericName, null, null) { }

        public MethodGenericInfo(string genericName, IEnumerable<TypeName> constrainedTypes) : this(genericName, null, constrainedTypes) { }

        public MethodGenericInfo(string genericName, TypeKind constrainedKind, IEnumerable<TypeName> constrainedTypes)
            : this(genericName, (TypeKind?)constrainedKind, constrainedTypes) { }

        public override int GetHashCode() => HashCode.Combine(GenericName, ConstrainedKind, ConstrainedTypes);

        public override bool Equals(object obj) => obj is MethodGenericInfo other && Equals(other);

        public bool Equals(MethodGenericInfo other) 
            => GenericName.Equals(other.GenericName) && 
               ConstrainedKind.Equals(other.ConstrainedKind) && 
               ConstrainedTypes.SequenceEqual(other.ConstrainedTypes);
    }
}
