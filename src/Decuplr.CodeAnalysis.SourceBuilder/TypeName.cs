using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis.SourceBuilder {

    public readonly struct TypeName : IEquatable<TypeName> {

        private readonly string _name;
        private readonly string _namespace;
        private readonly string[] _parentNames;

        public string Namespace => _namespace ?? string.Empty;

        public IReadOnlyList<string> ParentNames => _parentNames ?? Array.Empty<string>();

        public string Name => _name ?? string.Empty;

        public bool IsGenericArgument { get; }

        internal static string[] VerifyChainedIndentifier(string fullTypeName, string argName) {
            var sliced = fullTypeName.Split('.');
            if (sliced.Any(x => !SyntaxFacts.IsValidIdentifier(x)))
                throw new ArgumentException($"Invalid type identification name : {fullTypeName}", argName);
            return sliced;
        }

        public TypeName(ITypeSymbol symbol) {
            _name = symbol.Name;
            _parentNames = parentNames(symbol).Reverse().ToArray();
            _namespace = symbol.ContainingNamespace.ToString();
            IsGenericArgument = false;

            static IEnumerable<string> parentNames(ITypeSymbol symbol) {
                var currentSymbol = symbol;
                while(currentSymbol.ContainingType is { }) {
                    yield return currentSymbol.Name;
                    currentSymbol = currentSymbol.ContainingType;
                }
            }
        }

        internal TypeName(Type type) {
            _name = type.Name;
            _parentNames = parentNames(type).Reverse().ToArray();
            _namespace = type.Namespace;
            IsGenericArgument = false;

            static IEnumerable<string> parentNames(Type type) {
                var currentType = type;
                while(currentType.DeclaringType is { }) {
                    yield return currentType.Name;
                    currentType = currentType.DeclaringType;
                }
            }
        }

        internal TypeName(GeneratingTypeName typeName) {
            _namespace = typeName.Namespace;
            _name = typeName.TypeName;
            _parentNames = typeName.Parents.Select(x => x.ParentName).ToArray();
            IsGenericArgument = false;
        }

        private TypeName(bool isGeneric, string namespaceName, IEnumerable<string> parentNames, string typeName) {
            IsGenericArgument = isGeneric;
            _namespace = namespaceName;
            _name = typeName;
            _parentNames = parentNames.ToArray();
        }

        public TypeName(string namespaceName, string typeName) {
            VerifyChainedIndentifier(namespaceName, nameof(namespaceName));
            var slicedTypeName = VerifyChainedIndentifier(typeName, nameof(typeName));
            _namespace = namespaceName;
            _name = slicedTypeName[slicedTypeName.Length - 1];

            Array.Resize(ref slicedTypeName, slicedTypeName.Length - 1);
            _parentNames = slicedTypeName;
            IsGenericArgument = false;
        }

        public TypeName(string namespaceName, string parentNames, string typeName)
            : this(namespaceName, typeName) {
            _parentNames = VerifyChainedIndentifier(parentNames, nameof(parentNames));
        }

        public TypeName(string namespaceName, IEnumerable<string> parentNames, string typeName)
            : this(namespaceName, typeName) {
            _parentNames = parentNames.ToArray();
            if (_parentNames.Any(x => !SyntaxFacts.IsValidIdentifier(x)))
                throw new ArgumentException("Invalid parent type name", nameof(parentNames));
        }

        public string GetFullName() {
            if (ParentNames.Count == 0)
                return $"{Namespace}.{Name}";
            return $"{Namespace}.{string.Join(".", ParentNames)}.{Name}";
        }

        public override string ToString() => GetFullName();
        public override bool Equals(object obj) => obj is TypeName otherName && Equals(otherName);
        public override int GetHashCode() => HashCode.Combine(Name, Namespace, ParentNames);

        public static implicit operator string(TypeName name) => name.ToString();
        public static implicit operator TypeName(Type type) => FromType(type);

        public static TypeName FromGenericArgument(string genericName) => new TypeName(true, string.Empty, Enumerable.Empty<string>(), genericName);
        public static TypeName FromType(Type type) => new TypeName(type);
        public static TypeName FromType(ITypeSymbol symbol) {
            if (symbol is ITypeParameterSymbol paramSybol)
                return new TypeName(true, string.Empty, Enumerable.Empty<string>(), paramSybol.Name);
            return new TypeName(symbol);
        }

        public bool Equals(TypeName other) => other.Name.Equals(Name) && other.Namespace.Equals(Namespace) && other.ParentNames.Equals(ParentNames);
    }
}
