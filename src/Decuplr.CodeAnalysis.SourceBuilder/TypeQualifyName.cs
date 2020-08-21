using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.SourceBuilder {

    public readonly struct TypeQualifyName {

        private readonly string _typeName;
        private readonly string _namespace;
        private readonly string[] _parentNames;

        public bool IsEmpty => _typeName is null;

        public string Namespace => _namespace ?? string.Empty;

        public IReadOnlyList<string> ParentNames => _parentNames ?? Array.Empty<string>();

        public string TypeName => _typeName ?? string.Empty;

        public bool IsGeneric { get; }

        internal static string[] VerifyChainedIndentifier(string fullTypeName, string argName) {
            var sliced = fullTypeName.Split('.');
            if (sliced.Any(x => !SyntaxFacts.IsValidIdentifier(x)))
                throw new ArgumentException($"Invalid type identification name : {fullTypeName}", argName);
            return sliced;
        }

        public TypeQualifyName(ITypeSymbol symbol) {
            _typeName = symbol.Name;
            _parentNames = parentNames(symbol).ToArray();
            _namespace = symbol.ContainingNamespace.ToString();
            IsGeneric = false;

            static IEnumerable<string> parentNames(ITypeSymbol symbol) {
                if (symbol.ContainingType is null)
                    yield break;
                yield return symbol.ContainingType.Name;
            }
        }

        internal TypeQualifyName(GeneratingTypeName typeName) {
            _namespace = typeName.Namespace;
            _typeName = typeName.TypeName;
            _parentNames = typeName.Parents.Select(x => x.ParentName).ToArray();
            IsGeneric = false;
        }

        public TypeQualifyName(string namespaceName, string typeName) {
            VerifyChainedIndentifier(namespaceName, nameof(namespaceName));
            var slicedTypeName = VerifyChainedIndentifier(typeName, nameof(typeName));
            _namespace = namespaceName;
            _typeName = slicedTypeName[slicedTypeName.Length - 1];

            Array.Resize(ref slicedTypeName, slicedTypeName.Length - 1);
            _parentNames = slicedTypeName;
            IsGeneric = false;
        }

        public TypeQualifyName(string namespaceName, string parentNames, string typeName)
            : this(namespaceName, typeName) {
            _parentNames = VerifyChainedIndentifier(parentNames, nameof(parentNames));
        }

        public TypeQualifyName(string namespaceName, string[] parentNames, string typeName)
            : this(namespaceName, typeName) {
            _parentNames = parentNames;
            if (_parentNames.Any(x => !SyntaxFacts.IsValidIdentifier(x)))
                throw new ArgumentException("Invalid parent type name", nameof(parentNames));
        }

        public string GetFullName() {
            if (ParentNames.Count == 0)
                return $"{Namespace}.{TypeName}";
            return $"{Namespace}.{string.Join(".", ParentNames)}.{TypeName}";
        }

        public override string ToString() => GetFullName();
        public static implicit operator string(TypeQualifyName name) => name.ToString();

        public static TypeQualifyName FromGeneric(string genericName) {

        }
    }
}
