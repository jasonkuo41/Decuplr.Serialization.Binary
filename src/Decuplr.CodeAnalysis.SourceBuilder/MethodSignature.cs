using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Serialization.SourceBuilder {

    public class MethodSignatureBuilder {

        private readonly Accessibility _accessibility;
        private readonly string _methodName;
        private readonly string _fullTypeName;
        private readonly List<MethodArg> _arguments = new List<MethodArg>();

        private MethodSignatureBuilder(string fullTypeName, string methodName) {
            if (!SyntaxFacts.IsValidIdentifier(methodName))
                throw new ArgumentException($"Invalid method name '{methodName}'.", nameof(methodName));
            _methodName = methodName;
            _fullTypeName = fullTypeName;
        }

        public static MethodSignatureBuilder CreateMethod(INamedTypeSymbol type, string methodName)
            => new MethodSignatureBuilder(type.ToString(), methodName);

        public static MethodSignatureBuilder CreateMethod(TypeQualifyName typeName, string methodName)
            => new MethodSignatureBuilder(typeName.ToString(), methodName);

        public static MethodSignature CreateConstructor(TypeQualifyName typeName, IEnumerable<MethodArg> args)
            => CreateConstructor(Accessibility.Public, typeName, args.AsEnumerable());
        public static MethodSignature CreateConstructor(TypeQualifyName typeName, params MethodArg[] args)
            => CreateConstructor(Accessibility.Public, typeName, args.AsEnumerable());

        public static MethodSignature CreateConstructor(Accessibility accessibility, TypeQualifyName typeName, params MethodArg[] args)
            => CreateConstructor(accessibility, typeName, args.AsEnumerable());

        public static MethodSignature CreateConstructor(Accessibility accessibility, TypeQualifyName typeName, IEnumerable<MethodArg> args)
            => new MethodSignature(accessibility, typeName.ToString(), null, args, isConstructor: true, RefKind.None);

        public MethodSignatureBuilder AddArgument(MethodArg arg) {
            _arguments.Add(arg);
            return this;
        }

        public MethodSignatureBuilder AddArgument(params MethodArg[] args) {
            _arguments.AddRange(args);
            return this;
        }

        public MethodSignature WithReturn(ITypeSymbol symbol) => WithReturn(RefKind.None, symbol);

        public MethodSignature WithReturn(RefKind refKind, ITypeSymbol symbol)
            => new MethodSignature(_accessibility, _methodName, symbol, _arguments, false, refKind);
    }

    public class MethodSignature {
        
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
        public ITypeSymbol? ReturnType { get; }

        /// <summary>
        /// The arguments this method contains
        /// </summary>
        public IReadOnlyList<MethodArg> Arguments { get; }

        internal MethodSignature(Accessibility accessibility, string methodName, ITypeSymbol? returnType, IEnumerable<MethodArg> args, bool isConstructor, RefKind returnRefKind) {
            if (returnRefKind != RefKind.None && returnRefKind != RefKind.Ref)
                throw new ArgumentException($"Invalid returning ref kind {returnRefKind}");
            Accessibility = accessibility;
            MethodName = methodName;
            ReturnType = returnType;
            IsConstructor = isConstructor;
            ReturnRefKind = returnRefKind;
            Arguments = args.ToList();
        }

        private string ApplyModifier(int i, string argName) {
            if (i > Arguments.Count)
                throw new ArgumentOutOfRangeException();
            var modifier = Arguments[i].RefKind;
            if (modifier == RefKind.None)
                return argName;
            if (modifier == RefKind.In || modifier == RefKind.RefReadOnly)
                return $"in {argName}";
            return $"{modifier.ToString().ToLowerInvariant()} {argName}";
        }

        /// <summary>
        /// Gets the invocation string, without target object
        /// </summary>
        /// <remarks>
        /// For example for method : <i>int MyMethod(int value, in double data)</i>, this returns : <i>MyMethod(a, in b)</i>
        /// . For constructors, this returns <i>new MyMethod(a, in b)</i>
        /// </remarks>
        /// <returns></returns>
        public string GetInvocationString(IEnumerable<string> argumentNames) {
            
            var argList = argumentNames.ToList();
            // Check for default argument in the future
            if (argList.Count != Arguments.Count)
                throw new ArgumentException($"Invalid argument count, passed {argList.Count}, but expected {Arguments.Count}.");

            var builder = new StringBuilder();
            if (IsConstructor)
                builder.Append("new ");
            builder.Append(MethodName);
            builder.Append('(');
            builder.Append(string.Join(",", GetArgName(argumentNames)));
            builder.Append(')');
            return builder.ToString();

            IEnumerable<string> GetArgName(IEnumerable<string> argNames) {
                var i = 0;
                foreach (var arg in argNames) {
                    yield return ApplyModifier(i, arg);
                    ++i;
                }
            }
        }

        public string CreateMethodHeader(Accessibility accessibility) {

        }
    }

}
