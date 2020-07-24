using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;

#if HIGH_LEVEL

namespace Decuplr.Serialization.SourceBuilder {

    public struct ArgumentType {

        public ArgumentType(ITypeSymbol symbol) {
            symbol.Equals()
        }

        public ArgumentType(Type type) {

        }

        public string FullName { get; }
        public string ShortName { get; }

        public static implicit operator ArgumentType(Type type) {

        }

        public static ArgumentType FromSymbol(ITypeSymbol symbol) => new ArgumentType(symbol);
    }

    public struct MethodArguments {
        public MethodArguments(IParameterSymbol parameter) {

        }
    }

    public class TypeBuilder {

        private struct Field {
            public Accessibility Accessibility { get; set; }
            public ArgumentType ArgumentType { get; set; }
            public string FieldName { get; set; }
        }

        private struct AutoProperty {
            public Accessibility GetterAccessibility { get; set; }
            public Accessibility SetterAccessibility { get; set; }
            public ArgumentType ArgumentType { get; set; }
            public string PropertyName { get; set; }
        }

        private struct Method {

        }

        private readonly TypeKind _kind;
        private readonly string _typeName;
        private readonly string _typeNamespace;
        private readonly List<string> _usingNamespaces = new List<string>();
        private readonly List<ArgumentType> _inherits = new List<ArgumentType>();
        private readonly List<Field> _fields = new List<Field>();
        private readonly List<AutoProperty> _autoProperties = new List<AutoProperty>();

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

        public TypeBuilder Inherit(ArgumentType type, params ArgumentType[] otherTypes) {
            _inherits.Add(type);
            _inherits.AddRange(otherTypes);
            return this;
        }

        public TypeBuilder AddMethod(Accessibility accessibility, string methodName, Func<MethodSignatureBuilder, MethodSignature> sigBuilder, Action<CodeNodeBuilder, MethodArguments[]> methodBuilder) {
            var builder = MethodSignature.Create(accessibility, methodName);
            var signature = sigBuilder(builder);


            return this;
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
            _fields.Add(new Field { Accessibility = accessibility, ArgumentType = type, FieldName = fieldName });
            return this;
        }

        public TypeBuilder AddProperty(ArgumentType type, string propertyName, Accessibility getterAccessibility, Accessibility setterAccessibility) {
            _autoProperties.Add(new AutoProperty { ArgumentType = type, PropertyName = propertyName, GetterAccessibility = getterAccessibility, SetterAccessibility = setterAccessibility });
            return this;
        }

    }

    public class MethodBuilder {
        
        public static MethodBuilder CreateMethod(MethodSignature signature, Action<CodeNodeBuilder, MethodArguments[]> methodBody) {

        }
    }

    public class GeneratedMethod {


    }
}

#endif