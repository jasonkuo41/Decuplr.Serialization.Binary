﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.SourceBuilder {

    public class MethodSignature {

        private string? _declaration;

        /// <summary>
        /// The reference kind this method returns
        /// </summary>
        public RefKind ReturnRefKind { get; }

        /// <summary>
        /// Is this method a constructor
        /// </summary>
        public bool IsConstructor { get; }

        /// <summary>
        /// The accessibility of this method
        /// </summary>
        public Accessibility Accessibility { get; }

        /// <summary>
        /// The name of the method, the full name of the type if this is a constructor
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// The returning type, maybe be void, null if constructor
        /// </summary>
        public TypeName ReturnType { get; }

        /// <summary>
        /// The containing type, equals to <see cref="ReturnType"/> if it's a constructor
        /// </summary>
        public TypeName ContainingType { get; }

        /// <summary>
        /// Contains all the generics of this method
        /// </summary>
        public IReadOnlyList<MethodGenericInfo> Generics { get; }

        /// <summary>
        /// The arguments this method contains
        /// </summary>
        public IReadOnlyList<MethodArg> Arguments { get; }

        private MethodSignature (Accessibility accessibility, TypeName constructingType, IEnumerable<MethodArg> args) {
            Accessibility = accessibility;
            MethodName = constructingType.ToString();
            ReturnType = constructingType;
            ContainingType = constructingType;
            IsConstructor = true;
            ReturnRefKind = RefKind.None;
            Generics = Array.Empty<MethodGenericInfo>();
            Arguments = args.ToList();
        }

        private MethodSignature(TypeName containingType, Accessibility accessibility, RefKind returnRefKind, TypeName returnType, IEnumerable<MethodGenericInfo> generics, string methodName, IEnumerable<MethodArg> args) {
            if (returnRefKind != RefKind.None && returnRefKind != RefKind.Ref)
                throw new ArgumentException($"Invalid returning ref kind {returnRefKind}");
            Accessibility = accessibility;
            MethodName = methodName;
            ReturnType = returnType;
            ContainingType = containingType;
            IsConstructor = false;
            ReturnRefKind = returnRefKind;
            Generics = generics.ToList();
            Arguments = args.ToList();
        }

        private MethodSignature(TypeName containingType, Accessibility accessibility, RefKind returnRefKind, TypeName returnType, IEnumerable<MethodGenericInfo> generics, string methodName, IEnumerable<MethodArg> args, bool isConstructor)  {
            Accessibility = accessibility;
            MethodName = methodName;
            ReturnType = returnType;
            ContainingType = containingType;
            IsConstructor = isConstructor;
            ReturnRefKind = returnRefKind;
            Generics = generics.ToList();
            Arguments = args.ToList();
        }

        internal MethodSignature CreateConstructor(Accessibility accessibility, TypeName constructingType, IEnumerable<MethodArg> args)
            => new MethodSignature(accessibility, constructingType, args);

        internal MethodSignature CreateMethod(TypeName containingType, Accessibility accessibility, RefKind returnRefKind, TypeName returnType, IEnumerable<MethodGenericInfo> generics, string methodName, IEnumerable<MethodArg> args)
            => new MethodSignature(containingType, accessibility, returnRefKind, returnType, generics, methodName, args);

        private string ApplyModifier(string argName, int i) {
            if (i > Arguments.Count)
                throw new ArgumentOutOfRangeException();
            var modifier = Arguments[i].RefKind;
            if (modifier == RefKind.None)
                return argName;
            if (modifier == RefKind.In || modifier == RefKind.RefReadOnly)
                return $"in {argName}";
            return $"{modifier.ToString().ToLowerInvariant()} {argName}";
        }

        private void CheckCountMatch(int actual, int expected, string argName) {
            if (actual != expected)
                throw new ArgumentOutOfRangeException(argName, actual, $"Argument count mismatch, expected {expected}");
        }

        private string GetInvocationStringInternal(string[]? genericArguments, string[] argumentNames) {
            genericArguments ??= Array.Empty<string>();
            var builder = new StringBuilder();
            if (IsConstructor)
                builder.Append("new ");
            builder.Append(MethodName);
            if (genericArguments.Length > 0) {
                Debug.Assert(!IsConstructor);
                builder.Append('<');
                builder.Append(string.Join(",", genericArguments));
                builder.Append('>');
            }
            builder.Append('(');
            builder.Append(string.Join(",", argumentNames.Select(ApplyModifier)));
            builder.Append(')');
            return builder.ToString();
        }

        private string GetInvocationString(string[] genericArguments, string[] argumentNames) {
            CheckCountMatch(genericArguments.Length, Generics.Count, nameof(genericArguments));
            CheckCountMatch(argumentNames.Length, Arguments.Count, nameof(argumentNames));
            genericArguments.EnsureValidIdentifiers();
            argumentNames.EnsureValidIdentifiers();

            return GetInvocationStringInternal(genericArguments, argumentNames);
        }

        /// <summary>
        /// The declartion part of the method, e.g. 'private int MyMethod(in MyType type, out ThatType that)'
        /// </summary>
        public string GetDeclarationString() {
            return _declaration ??= BuildString();

            string BuildString() {
                var str = new StringBuilder();
                str.Append(Accessibility.ToCodeString()); // private
                str.Append(' ');
                str.Append(ReturnType); // int
                str.Append(' ');
                if (!IsConstructor)
                    str.Append(MethodName); // MyMethod
                if (Generics.Count > 0) {
                    str.Append('<');
                    str.Append(string.Join(",", Generics.Select(x => x.GenericName)));
                    str.Append('>');
                }
                str.Append("(");
                str.Append(string.Join(", ", Arguments.Select(x => x.ToParamString())));
                str.Append(")");
                if (Generics.All(x => !x.HasConstrain))
                    return str.ToString();
                foreach(var generic in Generics) {
                    if (!generic.HasConstrain)
                        continue;
                    str.Append("where");
                    str.Append(' ');
                    str.Append(generic.GenericName);
                    str.Append(" : ");
                    str.Append(string.Join(",", GetGenericsArgs(generic)));
                    str.Append(' ');
                }
                return str.ToString();
            }

            static IEnumerable<string> GetGenericsArgs(MethodGenericInfo info) {
                if (info.ConstrainedKind != null) {
                    yield return info.ConstrainedKind switch
                    {
                        TypeKind.Struct => "struct",
                        TypeKind.Class => "class",
                        _ => throw new ArgumentException($"Invalid type kind '{info.ConstrainedKind}'")
                    };
                }
                foreach (var type in info.ConstrainedTypes) {
                    yield return type.ToString();
                }
            }
        }

        public string GetInvocationString(params string[] argumentNames) {
            // Check for default argument in the future
            CheckCountMatch(argumentNames.Length, Arguments.Count, nameof(argumentNames));
            argumentNames.EnsureValidIdentifiers();

            return GetInvocationStringInternal(null, argumentNames);
        }

        /// <summary>
        /// Gets the invocation string, without target object
        /// </summary>
        /// <remarks>
        /// For example for method : <i>int MyMethod(int value, in double data)</i>, this returns : <i>MyMethod(a, in b)</i>
        /// . For constructors, this returns <i>new MyMethod(a, in b)</i>
        /// </remarks>
        /// <returns></returns>
        public string GetInvocationString(IEnumerable<string> argumentNames) => GetInvocationString(argumentNames.ToArray());

        public string GetInvocationString(IEnumerable<string> genericArguments, params string[] argumentNames) => GetInvocationString(genericArguments.ToArray(), argumentNames);

        public string GetInvocationString(IEnumerable<string> genericArguments, IEnumerable<string> argumentNames) => GetInvocationString(genericArguments.ToArray(), argumentNames.ToArray());
        public MethodSignature Rename(string newName)
            => new MethodSignature(ContainingType, Accessibility, ReturnRefKind, ReturnType, Generics, newName, Arguments, IsConstructor);

        public MethodSignature Rename(Accessibility accessibility, string newName)
            => new MethodSignature(ContainingType, accessibility, ReturnRefKind, ReturnType, Generics, newName, Arguments, IsConstructor);

    }
}
