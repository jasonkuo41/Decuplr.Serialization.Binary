using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    /// <summary>
    /// Represents a structure that is capable of composing a sepcific member for serialize and deserialize
    /// </summary>
    public interface IMemberComposer {
        /// <summary>
        /// The parent type that composes the type
        /// </summary>
        ITypeComposer ParentType { get; }

        /// <summary>
        /// The target member that is being composed
        /// </summary>
        MemberMetaInfo TargetMember { get; }

        /// <summary>
        /// The composing structure's symbol
        /// </summary>
        ITypeSymbol ComposerSymbol { get; }

        IReadOnlyList<MethodSignature> Methods { get; }
    }

}
