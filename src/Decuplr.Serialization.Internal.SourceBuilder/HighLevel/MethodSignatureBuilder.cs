using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

#if HIGH_LEVEL

namespace Decuplr.Serialization.SourceBuilder {
    public class MethodSignatureBuilder {

        private readonly Accessibility _accessibility;
        private readonly string _methodName;
        private readonly List<MethodArguments> _arguments = new List<MethodArguments>();

        private MethodModifiers _modifiers;

        public MethodSignatureBuilder(Accessibility accessibility, string methodName) {
            _accessibility = accessibility;
            _methodName = methodName;
        }

        public MethodSignatureBuilder With(MethodModifiers modifiers) {
            _modifiers |= modifiers;
            return this;
        }

        public MethodSignatureBuilder Arguments(params MethodArguments[] arguments) {
            _arguments.AddRange(arguments);
            return this;
        }

        public MethodSignatureBuilder Arguments(params ITypeSymbol[] arguments) 
            => Arguments(arguments.Select(x => new MethodArguments(x)).ToArray());

        public MethodSignature Returns(ArgumentType type) {
            return new MethodSignature(_accessibility, type, _methodName, _modifiers, _arguments);
        }

        public MethodSignature Returns<TReturn>() => Returns(typeof(TReturn));

        public MethodSignature Returns(ITypeSymbol symbol) => Returns(new ArgumentType(symbol));
        public MethodSignature ReturnsVoid() => Returns(typeof(void));

        public static implicit operator MethodSignature (MethodSignatureBuilder builder) => builder.ReturnsVoid();
    }
}

#endif