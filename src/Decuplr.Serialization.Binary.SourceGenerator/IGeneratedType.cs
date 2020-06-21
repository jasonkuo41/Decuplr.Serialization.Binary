using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary {
    internal interface IGeneratedType {
        /// <summary>
        /// Where the type should be placed in the final product
        /// </summary>
        GeneratedPlacement Placement { get; }

        /// <summary>
        /// The target type that results in the generation of this type, null if not applicable
        /// </summary>
        INamedTypeSymbol? TargetType { get; }

        /// <summary>
        /// Is this a struct or a class
        /// </summary>
        GeneratedTypeKind TypeKind { get; }

        /// <summary>
        /// The purpose of this type, what is it for?
        /// </summary>
        GeneratedPurpose Purpose { get; }

        /// <summary>
        /// The name of the generated type
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// The containing source code for the generated type
        /// </summary>
        string SourceCode { get; }

        /// <summary>
        /// The suposed output file name, empty or null if not applicable
        /// </summary>
        string DesiredFilename { get; }

        /// <summary>
        /// The info of the parser that this type supplies, empty if it does not inherit TypeParser, TypeParserProvider or GenericParserProvider
        /// </summary>
        IReadOnlyCollection<ParserInfo> Parsers { get; }
    }
}
