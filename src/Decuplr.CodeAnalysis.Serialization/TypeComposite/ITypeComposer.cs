using System.Collections.Generic;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {

    /// <summary>
    /// Represents a structure that is capable of composing a type with <see cref="IMemberComposer"/>
    /// </summary>
    public interface ITypeComposer {
        /// <summary>
        /// The layout that this composer represents
        /// </summary>
        SchemaLayout SourceSchema { get; }

        /// <summary>
        /// The symbol that the compser respresents
        /// </summary>
        ITypeSymbol ComposerSymbol { get; }

        /// <summary>
        /// The member composers of this type composer, indexed by their property name
        /// </summary>
        IReadOnlyDictionary<string, IMemberComposer> MemberComposers { get; }

        /// <summary>
        /// Methods that this signature presents
        /// </summary>
        IReadOnlyList<MethodSignature> Methods { get; }
    }

}
