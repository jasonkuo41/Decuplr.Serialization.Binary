using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary {

    internal struct ParserInfo {
        public IReadOnlyList<string> ParserNamespaces { get; set; }
        public IParserKindProvider ParserKind { get; set; }
    }

    internal class GeneratedTypeBuilder : IGeneratedType {

        private readonly List<string> namespaces = new List<string>();
        private readonly List<ParserInfo> parsers = new List<ParserInfo>();

        public GeneratedTypeKind TypeKind { get; set; }

        /// <summary>
        /// The purpose of this type
        /// </summary>
        public GeneratedPurpose Purpose { get; set; }

        /// <summary>
        /// Where the type should be placed
        /// </summary>
        public GeneratedPlacement Placement { get; set; }

        public INamedTypeSymbol TargetType { get; }

        public IReadOnlyCollection<ParserInfo> Parsers => parsers;

        public string TypeName { get; }

        public string Category { get; set; } = string.Empty;

        public string DesiredFilename { get; set; } = string.Empty;

        public string SourceCode { get; }

        public bool HasModifiedConstructor { get; set; }

        public GeneratedTypeBuilder(INamedTypeSymbol targetType, string name, GeneratedTypeKind kind, GeneratedPurpose purpose, GeneratedPlacement placement, string sourceCode) {
            TypeKind = kind;
            Purpose = purpose;
            Placement = placement;
            TargetType = targetType;
            TypeName = name;
            SourceCode = sourceCode;
        }

    }

    internal enum GeneratedTypeKind : byte {
        Other,
        Class,
        Struct,
        Nested
    }

    internal enum GeneratedPurpose {
        Other,
        /// <summary>
        /// The type is created for deserializing a target type
        /// </summary>
        Deserialization,

        /// <summary>
        /// The type is created for serializing a target type
        /// </summary>
        Serialization,

        /// <summary>
        /// The type is a dependency (a combination of ways to serialize a member) for deserialize / serializing a target type
        /// </summary>
        MemberDependency,

        /// <summary>
        /// The type is a collection of dependency to construct or deconstruct a type
        /// </summary>
        TypeConstructorArgs,

        /// <summary>
        /// The type is meant as a wrapper over other type
        /// </summary>
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
