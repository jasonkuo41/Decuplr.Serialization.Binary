using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal abstract class MemberChainedMethods<T> : IChainedMethods where T : MemberChainedMethods<T> {

        private readonly bool _hasChainedMethod;
        private readonly int? _currentIndex;
        private readonly Func<int?, MethodSignature> _nextMethod;

        public MethodSignature MethodSignature { get; }
        
        public MethodArg this[TypeName typeName] => Arguments.First(x => x.TypeName.Equals(typeName));

        public MethodArg this[TypeName typeName, int index] => Arguments.Where(x => x.TypeName.Equals(typeName)).ElementAt(index);

        public IReadOnlyList<MethodTypeParams> TypeParameters => MethodSignature.TypeParameters;

        public IReadOnlyList<MethodArg> Arguments => MethodSignature.Arguments;

        public bool HasChainedMethodInvoked { get; private set; }

        bool IChainedMethods.HasChainedMethod => _hasChainedMethod;

        public MemberChainedMethods(Func<int?, MethodSignature> nextMethodSignature) {
            _hasChainedMethod = true;
            _currentIndex = null;
            _nextMethod = nextMethodSignature;
            MethodSignature = nextMethodSignature(null);
        }

        protected MemberChainedMethods(T sourceMethods, bool hasNextMethod) {
            _hasChainedMethod = hasNextMethod;
            _currentIndex = sourceMethods._currentIndex.HasValue ? sourceMethods._currentIndex + 1 : 0;
            _nextMethod = sourceMethods._nextMethod;
            MethodSignature = _nextMethod(_currentIndex);
            MethodSignature = MethodSignature.Rename(Accessibility.Private, MethodSignature.MethodName);
        }

        private void EnsureHasChainedMethod() {
            if (!_hasChainedMethod)
                throw new InvalidOperationException("There's no chained method");
        }

        private string InvokeNextMethodCore(Action<IChainedMethodsModifier>? action) {
            EnsureHasChainedMethod();
            if (action is null)
                return MethodSignature.GetInvocationString(MethodSignature.TypeParameters.Select(x => x.GenericName), MethodSignature.Arguments.Select(x => x.ArgName));
            var modifier = new ChainedMethodModifier(MethodSignature);
            action(modifier);
            return modifier.GetInvocationString();
        }

        protected abstract T MoveNext(T sourceMethod, bool hasNextMethod);

        public string InvokeNextMethod() => InvokeNextMethodCore(null);

        public string InvokeNextMethod(Action<IChainedMethodsModifier> action) => InvokeNextMethodCore(action);

        public T MoveNext(bool hasNextMethod) {
            var us = this as T;
            Debug.Assert(us is { });
            return MoveNext(us, hasNextMethod);
        }
    }
}
