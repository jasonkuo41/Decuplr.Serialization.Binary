using System;
using System.Buffers;
using System.Linq;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.TypeComposite.Internal.ChainedMethods {
    internal class ChainedMethodModifier : IChainedMethodsModifier {

        private readonly MethodSignature _methodSignature;
        private readonly MethodArg[] _clonedArgs;

        public string this[TypeName typeName] {
            get => this[typeName, 0];
            set => this[typeName, 0] = value;
        }

        public string this[TypeName typeName, int index] {
            get => _clonedArgs.Where(x => x.TypeName.Equals(typeName)).ElementAt(index).ArgName;
            set {
                var currentIndex = 0;
                for (var i = 0; i < _clonedArgs.Length; ++i) {
                    ref var arg = ref _clonedArgs[i];
                    if (!arg.TypeName.Equals(typeName))
                        continue;
                    if (currentIndex != index) {
                        currentIndex++;
                        continue;
                    }
                    // Found index, assign back the value
                    arg = arg.Rename(value);
                    return;
                }
                throw new ArgumentException($"Unable to locate the type name '{typeName}' in the method signature '{_methodSignature.MethodName}'");
            }
        }

        public ChainedMethodModifier(MethodSignature signature) {
            _methodSignature = signature;
            _clonedArgs = signature.Arguments.ToArray();
        }

        public string GetInvocationString() 
            => _methodSignature.GetInvocationString(_methodSignature.TypeParameters.Select(x => x.GenericName), _clonedArgs.Select(x => x.ArgName));
    }
}
