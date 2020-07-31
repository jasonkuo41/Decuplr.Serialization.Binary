using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.Serialization.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal class MemberComposer : IMemberComposer {
        
        public ITypeComposer ParentType { get; }

        public MemberMetaInfo TargetMember { get; }

        public ITypeSymbol ComposerSymbol { get; }

        public IReadOnlyList<MethodSignature> Methods { get; }

        public MemberComposer(ITypeComposer type, MemberMetaInfo targetMember, ITypeSymbol composerSymbol, IReadOnlyList<MethodSignature> methods) {
            ParentType = type;
            TargetMember = targetMember;
            ComposerSymbol = composerSymbol;
            Methods = methods;
        }

    }
}
