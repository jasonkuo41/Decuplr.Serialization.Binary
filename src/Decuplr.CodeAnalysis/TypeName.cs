using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis {

    /// <summary>
    /// Represents a strict and unique name for any given <see cref="INamedTypeSymbol"/> and <see cref="Type"/>. 
    /// It is immutable and would not be affected by any modification to the compilation.
    /// </summary>
    public class TypeName : IEquatable<TypeName> {

        private static readonly ConcurrentDictionary<Type, TypeName> _typeCache = new ConcurrentDictionary<Type, TypeName>();

        private string? _runtimeName;
        private string? _codeStyleName;
        private int? _hashCode;
        private TypeName? _genericDefinition;

        /// <summary>
        /// The namespace that this type contains in
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// The containing type, null if none
        /// </summary>
        public TypeName? ContainingType { get; }

        /// <summary>
        /// The type's name without any generic annotations
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type argument for the generics
        /// </summary>
        public IReadOnlyList<TypeName> GenericArguments { get; }

        /// <summary>
        /// Is it a type parameter that exists as a generic type's parameter
        /// </summary>
        public bool IsGenericParameter { get; }

        /// <summary>
        /// Is the type generic
        /// </summary>
        public bool IsGeneric => GenericArguments.Count != 0;

        /// <summary>
        /// Is the type generic definition without any concrete generic arguments
        /// </summary>
        public bool IsGenericDefinition {
            get {
                if (IsGeneric && GenericArguments[0].IsGenericParameter) {
                    Debug.Assert(GenericArguments.All(x => x.IsGenericParameter));
                    return true;
                }
                // short circuit so that when isgeneric is true, we never evaluate more
                Debug.Assert(!IsGeneric || !GenericArguments.Any(x => x.IsGenericParameter));
                return false;
            }
        }

        /// <summary>
        /// The generic definition of this generic type, null if this is not a generic type
        /// </summary>
        public TypeName? GenericDefinition => !IsGeneric ? null : (_genericDefinition ??= new TypeName(Namespace, ContainingType, Name, GenericArguments));

        /// <summary>
        /// The coding style format of the type, for example System.Collections.Generic.<see cref="List{T}.Enumerator"/>.
        /// </summary>
        public string CodeStyleName {
            get {
                return _codeStyleName ??= GetCodeStyleName();

                string GetCodeStyleName() {
                    var builder = new StringBuilder();
                    builder.Append(Namespace);
                    builder.Append('.');
                    foreach (var containingType in GetParentNames()) {
                        containingType.AppendCodeStyleShortName(builder);
                        builder.Append('.');
                    }
                    AppendCodeStyleShortName(builder);
                    return builder.ToString();
                }
            }
        }

        /// <summary>
        /// The runtime style format of the type, can be used for <see cref="Type.GetType(string)"/> to retrieve the full type information. For example System.Collections.Generic.List`1+Enumerator[System.Int32].
        /// </summary>
        public string? RuntimeName {
            get {
                const string invalidName = "|";
                var result = _runtimeName ??= GetRuntimeNameActual();
                return result.Equals(invalidName) ? null : result;

                string GetRuntimeNameActual() {
                    if (IsGenericParameter)
                        return invalidName;
                    var builder = new StringBuilder();
                    builder.Append(Namespace);
                    builder.Append('.');

                    foreach (var parentType in GetParentNames()) {
                        parentType.AppendRuntimeShortName(builder);
                        builder.Append('+');
                    }

                    AppendRuntimeShortName(builder);

                    if (!TryAppendTypeArguments(builder))
                        return invalidName;

                    return builder.ToString();
                }

                bool TryAppendTypeArguments(StringBuilder builder) {
                    var names = new List<TypeName>();
                    foreach (var parentType in GetParentNames()) {
                        names.AddRange(parentType.GenericArguments);
                    }
                    names.AddRange(GenericArguments);
                    if (names.All(x => x.IsGenericParameter)) {
                        return true;
                    }
                    if (names.All(x => !x.IsGenericParameter)) {
                        builder.Append('[');
                        builder.Append(string.Join(',', names.Select(x => x.RuntimeName)));
                        builder.Append(']');
                        return true;
                    }
                    return false;
                }
            }
        }

        private TypeName(string namespaceName, TypeName? parent, string typeName, IEnumerable<TypeName>? typeArguments) {
            Namespace = namespaceName;
            ContainingType = parent;
            Name = typeName;
            GenericArguments = typeArguments?.ToArray() ?? Array.Empty<TypeName>();
            IsGenericParameter = false;
        }

        private TypeName(TypeName containingType, string typeName) {
            Namespace = string.Empty;
            ContainingType = containingType;
            Name = typeName;
            GenericArguments = Array.Empty<TypeName>();
            IsGenericParameter = true;
        }

        private static TypeName CreateFromType(Type type) {
            if (type.IsGenericParameter)
                return new TypeName(FromType(type.DeclaringType), type.Name);
            var parentType = type.DeclaringType is null ? null : _typeCache.GetOrAdd(type.DeclaringType, CreateFromType);
            return new TypeName(type.Namespace, parentType, type.Name, type.GetGenericArguments().Select(x => CreateFromType(x)));
        }

        public static TypeName FromType<T>() => FromType(typeof(T));

        public static TypeName FromType(Type type) {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            return _typeCache.GetOrAdd(type, CreateFromType);
        }

        public static TypeName FromType(ITypeSymbol symbol) {

            if (!(symbol is INamedTypeSymbol || symbol is IArrayTypeSymbol || symbol is ITypeParameterSymbol))
                throw new NotSupportedException($"ITypeSymbol Kind {symbol.GetType()} is not supported for TypeName");

            if (symbol is ITypeParameterSymbol paramSybol) {
                if (paramSybol.DeclaringType is null)
                    throw new NotSupportedException("Dangling type parameter is not supported for TypeName to correctly generate a unique name");
                return new TypeName(FromType(paramSybol.DeclaringType), symbol.Name);
            }

            var parentType = symbol.ContainingType is { } ? FromType(symbol.ContainingType) : null;
            var typeArgs = symbol is INamedTypeSymbol namedType ? namedType.TypeArguments.Select(x => FromType(x)) : null;

            return new TypeName(symbol.ContainingNamespace.ToString(), parentType, symbol.Name, typeArgs);
        }

        private IEnumerable<TypeName> GetParentNames() {
            return UnrollParentNames().Reverse();

            IEnumerable<TypeName> UnrollParentNames() {
                var parentNames = ContainingType;
                while (parentNames is { }) {
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
            builder.Append(string.Join(',', GenericArguments));
            builder.Append('>');
        }

        private void AppendRuntimeShortName(StringBuilder builder) {
            builder.Append(Name);
            if (IsGeneric) {
                builder.Append('`');
                builder.Append(GenericArguments.Count);
            }
        }

        public TypeName MakeGenericType(params TypeName[] typeNames) {
            if (!IsGenericDefinition)
                throw new InvalidOperationException($"{this} is not a GenericTypeDefinition. MakeGenericType may only be called on a type for which Type.IsGenericTypeDefinition is true.");
            if (typeNames.Length == GenericArguments.Count)
                throw new ArgumentException("The number of generic arguments provided doesn't equal the arity of the generic type definition.");
            return new TypeName(Namespace, ContainingType, Name, typeNames);
        }

        public TypeName MakeGenericType(IEnumerable<TypeName> typeNames) => MakeGenericType(typeNames.ToArray());

        public override string ToString() => CodeStyleName;

        public override bool Equals(object obj) => obj is TypeName otherName && Equals(otherName);

        public override int GetHashCode() {
            return _hashCode ??= CalcHashCode();

            int CalcHashCode() {
                var hashCode = new HashCode();
                hashCode.Add(IsGenericParameter);
                hashCode.Add(Namespace);
                hashCode.Add(Name);
                hashCode.Add(ContainingType);
                for (var i = 0; i < GenericArguments.Count; ++i)
                    hashCode.Add(GenericArguments[i]);
                return hashCode.ToHashCode();
            }
        }

        public bool Equals(TypeName other) {
            bool isEqual = true;
            if (IsGenericParameter && other.IsGenericParameter) {
                return Equals(ContainingType, other.ContainingType);
            }
            isEqual &= IsGenericParameter.Equals(other.IsGenericParameter);
            isEqual &= Namespace.Equals(other.Namespace);
            isEqual &= Name.Equals(other.Name);
            isEqual &= Equals(ContainingType, other.ContainingType);
            isEqual &= GenericArguments.SequenceEqual(other.GenericArguments);
            return isEqual;
        }

        public static TypeName Void { get; } = FromType(typeof(void));

    }
}
