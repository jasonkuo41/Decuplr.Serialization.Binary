using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.SourceBuilder {

    public class MethodSignatureBuilder {

        private readonly Accessibility _accessibility;
        private readonly string _methodName;
        private readonly List<MethodArg> _arguments;

        public static MethodSignatureBuilder CreateMethod(TypeQualifyName typeName, string methodName) {

        }

        public static MethodSignature CreateConstructor(TypeQualifyName typeName, IEnumerable<MethodArg> args)
            => new MethodSignature(fulltypeName, null, args, isConstructor: true, RefKind.None);

        public static MethodSignature CreateConstructor(TypeQualifyName typeName, params MethodArg[] args)
            => CreateConstructor(fulltypeName, args.AsEnumerable());

        public static MethodSignatureBuilder AppendArgument(MethodArg arg) {

        }

        public static MethodSignatureBuilder AppendArgument(params MethodArg[] args) {

        }

        public MethodSignature WithReturn(ITypeSymbol symbol) => WithReturn(RefKind.None, symbol);

        public MethodSignature WithReturn(RefKind refKind, ITypeSymbol symbol) {

        }
    }

    public class MethodSignature {
        
        public RefKind ReturnRefKind { get; }

        /// <summary>
        /// Is this method a constructor
        /// </summary>
        public bool IsConstructor { get; }

        /// <summary>
        /// The name of the method, the full name of the constructor if this is a constructor
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

        internal MethodSignature(string methodName, ITypeSymbol? returnType, IEnumerable<MethodArg> args, bool isConstructor, RefKind returnRefKind) {
            if (returnRefKind != RefKind.None && returnRefKind != RefKind.Ref)
                throw new ArgumentException($"Invalid returning ref kind {returnRefKind}");
            MethodName = methodName;
            ReturnType = returnType;
            IsConstructor = isConstructor;
            ReturnRefKind = returnRefKind;
            Arguments = args.ToList();
        }

        public static MethodSignature CreateMethod(string methodName, ITypeSymbol returnType, IEnumerable<MethodArg> args)
            => CreateMethod(methodName, returnType, RefKind.None, args);

        public static MethodSignature CreateMethod(string methodName, ITypeSymbol returnType, RefKind returnRefKind, IEnumerable<MethodArg> args)
            => new MethodSignature(methodName, returnType, args, isConstructor: false, returnRefKind);

        public static MethodSignature CreateMethod(string methodName, ITypeSymbol returnType, params MethodArg[] args)
            => CreateMethod(methodName, returnType, args.AsEnumerable());

        public static MethodSignature CreateConstructor(string fulltypeName, IEnumerable<MethodArg> args)
            => new MethodSignature(fulltypeName, null, args, isConstructor: true, RefKind.None);

        public static MethodSignature CreateConstructor(string fulltypeName, params MethodArg[] args)
            => CreateConstructor(fulltypeName, args.AsEnumerable());

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
