using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Serialization.SourceBuilder {
    public readonly struct ParentTypeNameInfo {

        private readonly string _parentName;

        internal bool Empty => _parentName is null;
        public string ParentName => _parentName ?? string.Empty;
        public TypeKind ParentKind { get; }

        public ParentTypeNameInfo(TypeKind parentKind, string parentName) {
            if (!SyntaxFacts.IsValidIdentifier(parentName))
                throw new ArgumentException($"{parentName} is not valid identifier", nameof(parentName));
            
            if (parentKind != TypeKind.Class && parentKind != TypeKind.Interface && parentKind != TypeKind.Struct)
                throw new ArgumentException($"TypeKind {parentKind} cannot be nested", nameof(parentKind));

            ParentKind = parentKind;
            
            _parentName = parentName;
        }

        public static implicit operator ParentTypeNameInfo((TypeKind, string) tuple) => new ParentTypeNameInfo(tuple.Item1, tuple.Item2);

        public void Deconstruct(out TypeKind kind, out string name) {
            name = ParentName;
            kind = ParentKind;
        }

        public override string ToString() => ParentName;
    }
}
