using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis {

    internal static class SyntaxToolkit {
        public static 
    }


    public class TypeName : IEquatable<TypeName> {

        public string Namespace { get; }

        public IReadOnlyList<TypeName> Parents { get; }

        public string Name { get; }

        internal static string[] VerifyNamespaces(string fullTypeName, string argName) {
            var sliced = fullTypeName.Split('.');
            if (sliced.Any(x => !SyntaxFacts.IsValidIdentifier(x.Trim())))
                throw new ArgumentException($"Invalid type identification name : {fullTypeName}", argName);
            return sliced;
        }

        internal static string[] VerifyTypeNames(string fullTypeNames, string argName) {
            var slicedType = fullTypeNames.Split('.');
            if (slicedType.Any(x => TypeWithClampedString(x).Any(x => !SyntaxFacts.IsValidIdentifier(x))))
                throw ThrowArgException();
            return slicedType;

            IEnumerable<string> TypeWithClampedString(string source) {
                var startBracket = source.IndexOf('<');
                yield return startBracket < 0 ? source.Trim() : source.Substring(0, startBracket).Trim();
                foreach (var clamped in GetClampedStrings(source))
                    yield return clamped;
            }

            IEnumerable<string> GetClampedStrings(string source) {
                var startBracket = source.IndexOf('<');
                var endBracket = source.LastIndexOf('>');
                if (startBracket < 0 ^ endBracket < 0)
                    throw ThrowArgException();
                if (startBracket < 0 || endBracket < 0)
                    return Enumerable.Empty<string>();
                var clampedString = source.Substring(startBracket + 1, endBracket - startBracket - 1);
                return clampedString.Split(',').Select(x => x.Trim());
            }

            Exception ThrowArgException() => new ArgumentException($"Invalid type identification name : {fullTypeNames}", argName);
        }

        public TypeName(ITypeSymbol symbol) {
            _name = symbol.Name;
            _parentNames = parentNames(symbol).Reverse().ToArray();
            _namespace = symbol.ContainingNamespace.ToString();
            IsGenericArgument = false;

            static IEnumerable<string> parentNames(ITypeSymbol symbol) {
                var currentSymbol = symbol;
                while (currentSymbol.ContainingType is { }) {
                    yield return currentSymbol.Name;
                    currentSymbol = currentSymbol.ContainingType;
                }
            }
        }

        internal TypeName(Type type) {
            // Throw if type contains generic args, we don't support them for now, since it is not used
            if (type.IsGenericType)
                throw GenericNotSupported();
            _name = type.Name;
            _parentNames = parentNames(type).Reverse().ToArray();
            _namespace = type.Namespace;
            IsGenericArgument = false;

            static IEnumerable<string> parentNames(Type type) {
                var currentType = type;
                while (currentType.DeclaringType is { }) {
                    if (currentType.DeclaringType.IsGenericType)
                        throw GenericNotSupported();
                    yield return currentType.Name;
                    currentType = currentType.DeclaringType;
                }
            }

            static Exception GenericNotSupported() => new NotSupportedException("Generic type is not supported");
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
            VerifyNamespaces(namespaceName, nameof(namespaceName));
            var slicedTypeName = VerifyTypeNames(typeName, nameof(typeName));
            _namespace = namespaceName;
            _name = slicedTypeName[slicedTypeName.Length - 1];

            Array.Resize(ref slicedTypeName, slicedTypeName.Length - 1);
            _parentNames = slicedTypeName;
            IsGenericArgument = false;
        }

        public TypeName(string namespaceName, string parentNames, string typeName)
            : this(namespaceName, typeName) {
            _parentNames = VerifyTypeNames(parentNames, nameof(parentNames));
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
        public bool Equals(TypeName other) => other.Name.Equals(Name)
                                              && other.Namespace.Equals(Namespace)
                                              && other.ParentNames.Equals(ParentNames);
        public override int GetHashCode() => HashCode.Combine(Name, Namespace, ParentNames);

        public static implicit operator string(TypeName name) => name.ToString();
        public static implicit operator TypeName(Type type) => FromType(type);

        public static TypeName FromType(Type type) => new TypeName(type);
        public static TypeName FromType<T>() => new TypeName(typeof(T));
        public static TypeName FromType(ITypeSymbol symbol) {
            if (symbol is ITypeParameterSymbol paramSybol)
                return new TypeName(true, string.Empty, Enumerable.Empty<string>(), paramSybol.Name);
            return new TypeName(symbol);
        }

        public static TypeName Void { get; } = new TypeName(typeof(void));

    }
}
