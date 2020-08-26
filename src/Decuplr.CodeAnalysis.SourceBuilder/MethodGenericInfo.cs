using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.SourceBuilder {

    public enum GenericConstrainKind {
        Reference,
        Struct,
        Unmanaged,
        NotNull,
    }

    public struct MethodGenericInfo : IEquatable<MethodGenericInfo> {
        public string GenericName { get; }
        public GenericConstrainKind? ConstrainedKind { get; }
        public IReadOnlyList<TypeName> ConstrainedTypes { get; }

        public bool HasConstrain => ConstrainedKind != null || ConstrainedTypes.Count > 0;

        private MethodGenericInfo(string genericName, GenericConstrainKind? constrainedKind, IEnumerable<TypeName>? constrainedTypes) {
            genericName.EnsureValidIdentifier();
            GenericName = genericName;
            ConstrainedKind = constrainedKind;
            ConstrainedTypes = constrainedTypes?.ToArray() ?? Array.Empty<TypeName>();
        }

        public MethodGenericInfo(string genericName) : this(genericName, null, null) { }

        public MethodGenericInfo(string genericName, IEnumerable<TypeName> constrainedTypes) : this(genericName, null, constrainedTypes) { }

        public MethodGenericInfo(string genericName, GenericConstrainKind constrainedKind, IEnumerable<TypeName> constrainedTypes)
            : this(genericName, (GenericConstrainKind?)constrainedKind, constrainedTypes) { }

        public override int GetHashCode() => HashCode.Combine(GenericName, ConstrainedKind, ConstrainedTypes);

        public override bool Equals(object obj) => obj is MethodGenericInfo other && Equals(other);

        public bool Equals(MethodGenericInfo other) 
            => GenericName.Equals(other.GenericName) && 
               ConstrainedKind.Equals(other.ConstrainedKind) && 
               ConstrainedTypes.SequenceEqual(other.ConstrainedTypes);
    }
}
