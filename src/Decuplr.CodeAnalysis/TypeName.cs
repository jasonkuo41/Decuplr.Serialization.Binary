using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis {

    internal static class SyntaxToolkit {
        public static 
    }

    /// <summary>
    /// Represents a light weight name counterpart of <see cref="TypeName"/> with looser comparsion rules.
    /// This class is meant for as a ease to use identifier and shall not be used to respresent any unique names of a type.
    /// </summary>
    public readonly struct LaxTypeName : IEquatable<LaxTypeName> {
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

        public string Namespace { get; }
        public IReadOnlyList<LaxTypeName> Parents { get; }
        public string Name { get; }
        public IReadOnlyList<LaxTypeName>? TypeParameters { get; }
        public IReadOnlyList<GenericName> TypeArguments { get; }
        public bool IsGeneric => TypeArguments.Count > 0;
        public bool IsGenericDefinition => IsGeneric && TypeParameters is null;
        public bool IsTypeParameter { get; }

        private static IEnumerable<LaxTypeName> GetParentNames(IEnumerable<string> parentNames, string namespaces) {
            if (parentNames is IReadOnlyCollection<string> c && c.Count == 0)
                return Enumerable.Empty<LaxTypeName>();
            var parents = parentNames.ToList();
            return parents.Select((parent, i) => new LaxTypeName(namespaces, parents.Take(i), parent));
        }


        private LaxTypeName(string typeParameterName) {
            Namespace = string.Empty;
            Parents = Array.Empty<LaxTypeName>();
            Name = typeParameterName;
            TypeParameters = null;
            TypeArguments = Array.Empty<GenericName>();
            IsTypeParameter = true;
        }

        private LaxTypeName(string namespaces, IEnumerable<LaxTypeName> parents, string typeName, IEnumerable<LaxTypeName>? typeParameters, IEnumerable<GenericName> typeArguments) {
            Namespace = namespaces;
            Parents = parents.ToList();
            Name = typeName;
            TypeParameters = typeParameters?.ToList();
            TypeArguments = typeArguments.ToList();
            IsTypeParameter = false;
        }

        public static LaxTypeName FromName(string namespaces, IEnumerable<string> parents, string typeName, params GenericName[] typeArguments) {
            return new LaxTypeName(namespaces, GetParentNames(parents, namespaces), typeName, null, typeArguments);
        }

        public static LaxTypeName FromName(string namespaces, IEnumerable<LaxTypeName> parents, string typeName, params LaxTypeName[] arguemtns) {

        }

        public LaxTypeName MakeGenericType(params LaxTypeName[] typeNames) {
            if (!IsGenericDefinition)
                throw new InvalidOperationException($"{this} is not a GenericTypeDefinition. MakeGenericType may only be called on a type for which {nameof(IsGenericDefinition)} is true.");
            if (typeNames.Length != TypeArguments.Count)
                throw new ArgumentException("The number of generic arguments provided doesn't equal the arity of the generic type definition.");
            return new LaxTypeName(Namespace, Parents, Name, typeNames, TypeArguments);
        }

        public LaxTypeName GetGenericDefinition() {
            if (!IsGeneric)
                throw new InvalidOperationException($"This operation is only valid on generic types.");
            return new LaxTypeName(Namespace, Parents, Name, null, TypeArguments);
        }

        public bool Equals(LaxTypeName otherName)
            => IsTypeParameter.Equals(otherName.IsTypeParameter)
            && Namespace.Equals(otherName.Namespace)
            && Name.Equals(otherName.Name)
            && Parents.SequenceEqual(otherName.Parents)
            && (TypeParameters?.Equals(otherName.TypeParameters) ?? otherName.TypeParameters is null)
            && TypeArguments.Count.Equals(TypeArguments.Count);

        public override bool Equals(object obj) => (obj is TypeName typeName && Equals(typeName.LaxName)) || (obj is LaxTypeName name && Equals(name));

        public override int GetHashCode() {
            var hashCode = new HashCode();
            hashCode.Add(IsTypeParameter);
            hashCode.Add(Namespace);
            for (var i = 0; i < Parents.Count; i++)
                hashCode.Add(Parents[i]);
            hashCode.Add(Name);
            if (TypeParameters is { }) {
                for (var i = 0; i < TypeParameters.Count ; ++i)
                   hashCode.Add(TypeParameters[i]);
            }
            hashCode.Add(TypeArguments.Count);
            return hashCode.ToHashCode();
        }

        public override string ToString() {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(Namespace)) {
                builder.Append(Namespace);
                builder.Append('.');
            }
            for (var i = 0; i < Parents.Count; ++i) {
                builder.Append(Parents[i].ToString());
                builder.Append('.');
            }
            builder.Append(Name);
            if (!IsGeneric)
                return builder.ToString();
            builder.Append('<');
            if (IsGenericDefinition)
                builder.Append(string.Join(", ", TypeArguments.Select(x => x.ToString())));
            else
                builder.Append(string.Join(", ", TypeParameters.Select(x => x.ToString())));
            builder.Append('>');
            return builder.ToString();
        }
    }

    /// <summary>
    /// Represents a strict and unique name for any given <see cref="INamedTypeSymbol"/> and <see cref="Type"/>. 
    /// It is immutable and would not be affected by any modification to the compilation.
    /// </summary>
    public class TypeName : IEquatable<TypeName> {

        private static readonly ConcurrentDictionary<Type, TypeName> _typeCache = new ConcurrentDictionary<Type, TypeName>();

        private string? _runtimeName;
        private string? _codeStyleName;
        private TypeName? _genericDefinition;

        public string Namespace { get; }

        public TypeName? ContainingType { get; }

        /// <summary>
        /// The type's name without any generic annotations
        /// </summary>
        public string Name { get; }

        public IReadOnlyList<TypeName> TypeArguments { get; }

        //public IReadOnlyList<GenericName> TypeParameters { get; }

        public bool IsGeneric => TypeParameters.Count != 0;

        public bool IsGenericDefinition => IsGeneric && TypeArguments.Count == 0;

        public bool IsTypeParameter { get; }

        public TypeName? GenericDefinition => !IsGeneric ? null : (_genericDefinition ??= new TypeName(Namespace, ContainingType, Name, TypeArguments, TypeParameters));

        public string CodeStyleName {
            get {
                return _codeStyleName ??= GetCodeStyleName();

                string GetCodeStyleName() {
                    var builder = new StringBuilder();
                    builder.Append(Namespace);
                    builder.Append('.');
                    for(var i = 0; i < ContainingType.Count; ++i) {
                        ContainingType[i].AppendCodeStyleShortName(builder);
                        builder.Append('.');
                    }
                    AppendCodeStyleShortName(builder);
                    return builder.ToString();
                }
            }
        }

        public string? RuntimeName {
            get {
                return IsTypeParameter ? null : _runtimeName ??= GetRuntimeNameActual();

                string GetRuntimeNameActual() {
                    var builder = new StringBuilder();
                    builder.Append(Namespace);
                    builder.Append('.');

                    foreach(var parentType in GetParentNames()) {
                        parentType.AppendRuntimeShortName(builder);
                        builder.Append('+');
                    }

                    AppendRuntimeShortName(builder);

                    if (TypeArguments.Count > 0) {
                        builder.Append('[');
                        builder.Append(string.Join(',', TypeArguments.Select(x => x.ToString())));
                        builder.Append(']');
                    }

                    return builder.ToString();
                }
            }
        }

        private TypeName(string namespaceName, TypeName parent, string typeName, IEnumerable<TypeName> typeArguments, IEnumerable<GenericName> typeParams) {
            Namespace = namespaceName;
            ContainingType = parent;
            Name = typeName;
            TypeArguments = typeArguments.ToList();
            TypeParameters = typeParams.ToList();
            IsTypeParameter = false;
        }

        private TypeName(TypeName containingType, string typeName) {
            Namespace = string.Empty;
            ContainingType = containingType;
            Name = typeName;
            TypeArguments = Array.Empty<TypeName>();
            TypeParameters = Array.Empty<GenericName>();
            IsTypeParameter = true;
        }


        [return: NotNullIfNotNull("type")]
        private static TypeName? CreateFromType(Type? type) {
            if (type is null)
                return null;
            return new TypeName(type.Namespace, CreateFromType(type), type.Name, type.GenericTypeArguments.Select(x => CreateFromType(x)), type.);
        }

        public static TypeName FromType<T>() => FromType(typeof(T));

        public static TypeName FromType(Type type) {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsGenericParameter)
                return new TypeName(FromType(type.DeclaringType), type.Name);
            return _typeCache.GetOrAdd(type, CreateFromType!);
        }

        public static TypeName FromType(ITypeSymbol symbol) {
            if (symbol is ITypeParameterSymbol paramSybol) {
                if (paramSybol.DeclaringType is null)
                    throw new NotSupportedException("Dangling type parameter is not supported for TypeName to correctly generate a unique name");
                return new TypeName(FromType(paramSybol.DeclaringType), symbol.Name);
            }
            Namespace = symbol.ContainingNamespace.ToString();
            ContainingType = GetParentSymbols(symbol).Reverse().Select(x => new TypeName(x)).ToList();
            Name = symbol.Name;

            static IEnumerable<ITypeSymbol> GetParentSymbols(ITypeSymbol symbol) {
                var currentSymbol = symbol;
                while (currentSymbol.ContainingType is { }) {
                    yield return currentSymbol.ContainingType;
                    currentSymbol = currentSymbol.ContainingType;
                }
            }
        }

        private IEnumerable<TypeName> GetParentNames() {
            return UnrollParentNames().Reverse();

            IEnumerable<TypeName> UnrollParentNames() {
                var parentNames = ContainingType;
                while(parentNames is { }) {
                    yield return parentNames;
                    parentNames = parentNames.ContainingType;
                }
            }
        }

        private void AppendCodeStyleShortName(StringBuilder builder) {
            builder.Append(Name);
            if (!IsGeneric)
                return;
            builder.Append('<');
            builder.Append(AppendingName());
            builder.Append('>');

            IEnumerable<string> AppendingName() => IsGenericDefinition ? TypeParameters.Select(x => x.ToString()) : TypeArguments.Select(x => x.ToString());
        }

        private void AppendRuntimeShortName(StringBuilder builder) {
            builder.Append(Name);
            if (IsGeneric) {
                builder.Append('`');
                builder.Append(TypeArguments.Count);
            }
        }

        public TypeName MakeGenericType(params TypeName[] typeNames) {
            if (!IsGenericDefinition)
                throw new InvalidOperationException($"{this} is not a GenericTypeDefinition. MakeGenericType may only be called on a type for which Type.IsGenericTypeDefinition is true.");
            if (typeNames.Length == TypeArguments.Count)
                throw new ArgumentException("The number of generic arguments provided doesn't equal the arity of the generic type definition.");
            return new TypeName(Namespace, ContainingType, Name, TypeArguments, TypeParameters);
        }

        public TypeName MakeGenericType(IEnumerable<TypeName> typeNames) => MakeGenericType(typeNames.ToArray());

        public TypeName(ITypeSymbol symbol) {
            Namespace = symbol.ContainingNamespace.ToString();
            ContainingType = GetParentSymbols(symbol).Reverse().Select(x => new TypeName(x)).ToList();
            Name = symbol.Name;

            static IEnumerable<ITypeSymbol> GetParentSymbols(ITypeSymbol symbol) {
                var currentSymbol = symbol;
                while (currentSymbol.ContainingType is { }) {
                    yield return currentSymbol.ContainingType;
                    currentSymbol = currentSymbol.ContainingType;
                }
            }
        }

        public override string ToString() => CodeStyleName;
        public override bool Equals(object obj) => obj is TypeName otherName && Equals(otherName);
        public bool Equals(TypeName other) => other.Name.Equals(Name)
                                              && other.Namespace.Equals(Namespace)
                                              && other.ParentNames.Equals(ParentNames);
        public override int GetHashCode() => HashCode.Combine(Name, Namespace, ParentNames);

        public static TypeName Void { get; } = FromType(typeof(void));

    }
}
