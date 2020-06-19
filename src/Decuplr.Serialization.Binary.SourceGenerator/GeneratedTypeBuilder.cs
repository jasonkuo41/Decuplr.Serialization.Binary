using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary {
    internal interface IGeneratedType {
        GeneratedPlacement Placement { get; }
        INamedTypeSymbol TargetType { get; }
        GeneratedTypeKind TypeKind { get; }
        GeneratedPurpose Purpose { get; }
        string? Category { get; }
        string Name { get; }
        string SourceCode { get; }
        string DesiredFilename { get; }

        /// <summary>
        /// The kind of the parser this type supplies, empty if not applicable
        /// </summary>
        IReadOnlyCollection<IParserKindProvider> ParserKinds { get; }
    }

    internal class GeneratedTypeBuilder : IGeneratedType {

        private readonly List<string> namespaces = new List<string>();
        private readonly List<IParserKindProvider> kinds = new List<IParserKindProvider>();

        public GeneratedTypeKind TypeKind { get; set; }

        public GeneratedPurpose Purpose { get; set; }

        public GeneratedPlacement Placement { get; set; }

        public IReadOnlyCollection<string> UsingNamespaces => namespaces;

        public INamedTypeSymbol TargetType { get; }

        public IReadOnlyCollection<IParserKindProvider> ParserKinds => kinds;

        public string Name { get; }

        public string Category { get; set; } = string.Empty;

        public string DesiredFilename { get; set; } = string.Empty;

        public string SourceCode { get; }

        public GeneratedTypeBuilder(INamedTypeSymbol targetType, string name, GeneratedTypeKind kind, GeneratedPurpose purpose, GeneratedPlacement placement, string sourceCode) {
            TypeKind = kind;
            Purpose = purpose;
            Placement = placement;
            TargetType = targetType;
            Name = name;
            SourceCode = sourceCode;
        }

        public void AddNamespace(string namespaceName) => namespaces.Add(namespaceName);
    }

    internal enum GeneratedTypeKind : byte {
        Other,
        Class,
        Struct
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
