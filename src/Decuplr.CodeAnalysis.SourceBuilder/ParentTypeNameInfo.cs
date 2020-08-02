using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.SourceBuilder {
    public readonly struct ParentTypeNameInfo {

        private readonly string? _parentName;
        private readonly string[]? _typeName;

        internal bool Empty => _parentName is null;
        public string ParentName => _parentName ?? string.Empty;
        public IReadOnlyList<string> TypeParameterNames => _typeName ?? Array.Empty<string>();
        public TypeKind ParentKind { get; }

        public ParentTypeNameInfo(TypeKind parentKind, string parentName) {
            if (!SyntaxFacts.IsValidIdentifier(parentName))
                throw new ArgumentException($"{parentName} is not valid identifier", nameof(parentName));

            if (parentKind != TypeKind.Class && parentKind != TypeKind.Interface && parentKind != TypeKind.Struct)
                throw new ArgumentException($"TypeKind {parentKind} cannot be nested", nameof(parentKind));

            ParentKind = parentKind;
            
            _parentName = parentName;
            _typeName = null;
        }

        public ParentTypeNameInfo(TypeKind parentKind, string parentName, params string[] typeParameterNames)
            : this (parentKind, parentName, typeParameterNames.AsEnumerable()) {
        }

        public ParentTypeNameInfo(TypeKind parentKind, string parentName, IEnumerable<string> typeParameterNames)
            : this(parentKind, parentName) {

            var unmatched = typeParameterNames.Where(x => !SyntaxFacts.IsValidIdentifier(x));
            if (typeParameterNames.Any())
                throw new ArgumentException($"'{string.Join(",", typeParameterNames)}' contains invalid indentifier", nameof(typeParameterNames));

            _typeName = typeParameterNames.ToArray();
        }

        public ParentTypeNameInfo(INamedTypeSymbol symbol)
            : this (symbol.TypeKind, symbol.Name) {

            _typeName = symbol.TypeArguments.Select(x => x.ToString()).ToArray();
        }

        public static implicit operator ParentTypeNameInfo((TypeKind, string) tuple) => new ParentTypeNameInfo(tuple.Item1, tuple.Item2);

        public void Deconstruct(out TypeKind kind, out string name, out IReadOnlyList<string> typeParams) {
            name = ParentName;
            kind = ParentKind;
            typeParams = TypeParameterNames;
        }

        public override string ToString() => ParentName;
    }
}
