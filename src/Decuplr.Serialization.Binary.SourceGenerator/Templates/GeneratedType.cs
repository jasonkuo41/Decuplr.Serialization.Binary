using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.Templates {
    internal class GeneratedType {

        public GeneratedTypeKind TypeKind { get; }

        public GeneratedPurpose Purpose { get; }

        public GeneratedPlacement Target { get; }

        public IReadOnlyCollection<string> UsingNamespaces { get; }

        public string TypeName { get; }

        public INamedTypeSymbol TargetType { get; }

        public string? Category { get; }

        public string SourceCode { get; }
    }

    internal enum GeneratedTypeKind : byte {
        Other,
        Class,
        Struct,
        Function
    }

    internal enum GeneratedPurpose {
        Other,
        Deserialization,
        Serialization,
        Dependency,
        Wrapper,
    }

    internal enum GeneratedPlacement {
        /// <summary>
        /// It's a partial type that is an extension of the target type
        /// </summary>
        PartialExtension,

        /// <summary>
        /// The type should be seen as a nested type of the target type
        /// </summary>
        PartialNestedExtension,

        /// <summary>
        /// It should be a nested type of the generated entry type
        /// </summary>
        EntryClassNested,

        /// <summary>
        /// The target type is a standalone type
        /// </summary>
        Standalone
    }
}
