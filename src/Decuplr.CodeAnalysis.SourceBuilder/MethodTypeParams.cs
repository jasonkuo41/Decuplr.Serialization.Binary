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

    public struct MethodTypeParams : IEquatable<MethodTypeParams> {
        public string GenericName { get; }
        public GenericConstrainKind? ConstrainedKind { get; }
        public IReadOnlyList<TypeName> ConstrainedTypes { get; }

        public bool HasConstrain => ConstrainedKind != null || ConstrainedTypes.Count > 0;

        private MethodTypeParams(string genericName, GenericConstrainKind? constrainedKind, IEnumerable<TypeName>? constrainedTypes) {
            genericName.EnsureValidIdentifier();
            GenericName = genericName;
            ConstrainedKind = constrainedKind;
            ConstrainedTypes = constrainedTypes?.ToArray() ?? Array.Empty<TypeName>();
        }

        public MethodTypeParams(string genericName) : this(genericName, null, null) { }

        public MethodTypeParams(string genericName, IEnumerable<TypeName> constrainedTypes) : this(genericName, null, constrainedTypes) { }

        public MethodTypeParams(string genericName, GenericConstrainKind constrainedKind, IEnumerable<TypeName> constrainedTypes)
            : this(genericName, (GenericConstrainKind?)constrainedKind, constrainedTypes) { }

        public override int GetHashCode() => HashCode.Combine(GenericName, ConstrainedKind, ConstrainedTypes);

        public override bool Equals(object obj) => obj is MethodTypeParams other && Equals(other);

        public bool Equals(MethodTypeParams other) 
            => GenericName.Equals(other.GenericName) && 
               ConstrainedKind.Equals(other.ConstrainedKind) && 
               ConstrainedTypes.SequenceEqual(other.ConstrainedTypes);
    }
}
