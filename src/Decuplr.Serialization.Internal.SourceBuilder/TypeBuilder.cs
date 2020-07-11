using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.SourceBuilder {

    public struct ArgumentType {

        public ArgumentType(ITypeSymbol symbol) {

        }

        public static implicit operator ArgumentType(Type type) {

        }

    }

    public struct MethodArguments {
        public MethodArguments(IParameterSymbol parameter) {

        }
    }

    public class TypeBuilder {

        private struct Field {

            public Field(ArgumentType type, string fieldName) {
                ArgumentType = type;
                FieldName = fieldName;
            }

            public ArgumentType ArgumentType { get; }
            public string FieldName { get; }
        }

        private readonly TypeKind _kind;
        private readonly string _typeName;
        private readonly string _typeNamespace;
        private readonly List<string> _usingNamespaces = new List<string>();
        private readonly List<ArgumentType> _inherits = new List<ArgumentType>();

        private TypeBuilder(TypeKind kind, string typeName, string typeNamespace) {
            EnsureSupportKind(kind);
            EnsureTypeNameSimple(typeName);

            _kind = kind;
            _typeName = typeName;
            _typeNamespace = typeNamespace;

            static void EnsureSupportKind(TypeKind kind) {
                switch (kind) {
                    case TypeKind.Class:
                    case TypeKind.Struct:
                        return;
                    default:
                        throw new ArgumentException($"{kind} is not a valid Type for creation", nameof(kind));
                }
            }
            // We only perform check if type contains . so we don't mess up type name and namespace
            static void EnsureTypeNameSimple(string typeName) {
                if (typeName.Contains('.'))
                    throw new ArgumentException($"Name {typeName} is an invalid name of a type", nameof(typeName));
            }
        }

        public static TypeBuilder Create(TypeKind kind, string typeName, string typeNamespace) => new TypeBuilder(kind, typeName, typeNamespace);

        public static TypeBuilder CreatePartialExtension(ITypeSymbol symbol) => new TypeBuilder(symbol.TypeKind, symbol.Name, symbol.ContainingNamespace.ToString());

        public TypeBuilder UsingNamespace(string usingNamespace) {
            _usingNamespaces.Add(usingNamespace);
            return this;
        }

        public TypeBuilder UsingNamespace(params string[] usingNamespace) {
            _usingNamespaces.AddRange(usingNamespace);
            return this;
        }

        public TypeBuilder Inherit<TType>() => Inherit(typeof(TType));

        public TypeBuilder Inherit(ArgumentType type) {
            _inherits.Add(type);
            return this;
        }

        public TypeBuilder Inherit(params ArgumentType[] type) {
            _inherits.AddRange(type);
            return this;
        }

        public TypeBuilder AddMethod(Accessibility accessibility, string methodName, Func<MethodSignatureBuilder, MethodSignature> sigBuilder, Action<CodeNodeBuilder, MethodArguments[]> methodBuilder) {

        }

        public TypeBuilder AddMethod(MethodSignature signature, Action<CodeNodeBuilder, MethodArguments[]> methodBody) {

        }

        public TypeBuilder AddField<TType>(string fieldName)
            => AddField<TType>(fieldName, Accessibility.Private);

        public TypeBuilder AddField<TType>(string fieldName, Accessibility accessibility)
            => AddField(typeof(TType), fieldName, accessibility);

        public TypeBuilder AddField(ArgumentType type, string fieldName)
            => AddField(type, fieldName, Accessibility.Private);

        public TypeBuilder AddField(ArgumentType type, string fieldName, Accessibility accessibility) {

        }

        public TypeBuilder AddProperty(ArgumentType type, string name, Accessibility getterAccessibility, Accessibility setterAccessibility) {

        }

    }

    public class MethodBuilder {
        
        public static MethodBuilder CreateMethod(MethodSignature signature, Action<CodeNodeBuilder, MethodArguments[]> methodBody) {

        }
    }
}
