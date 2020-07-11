using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.SourceBuilder {
    public class MethodSignature {
        public Accessibility Accessibility { get; }

        public ArgumentType ReturnType { get; }

        public MethodModifiers Modifiers { get; }

        public string MethodName { get; }

        public IReadOnlyList<MethodArguments> Arguments { get; }

        public MethodSignature(Accessibility accessibility, ArgumentType returnType, string methodName, MethodModifiers modifiers, IReadOnlyList<MethodArguments> arguments) {
            Accessibility = accessibility;
            ReturnType = returnType;
            Arguments = arguments;
            MethodName = methodName;
            Modifiers = modifiers;
        }

        public static MethodSignatureBuilder Create(Accessibility accessibility, string methodName)
            => new MethodSignatureBuilder(accessibility, methodName);

        public static MethodSignature Create(IMethodSymbol symbol) {
            var modifiers = MethodModifiers.None;
            modifiers |= symbol.ContainingType.TypeKind == TypeKind.Interface ? MethodModifiers.None : MethodModifiers.Override;
            
            if (symbol.IsStatic)
                throw new ArgumentException($"{symbol} is a invalid method target", nameof(symbol));

            return new MethodSignature(symbol.DeclaredAccessibility, new ArgumentType(symbol.ReturnType), symbol.Name, modifiers, symbol.Parameters.Select(x => new MethodArguments(x)).ToList());
        }
        
    }
}
