using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.SourceBuilder {

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

    }

}
