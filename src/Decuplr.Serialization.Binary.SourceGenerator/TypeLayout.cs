using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal struct TypeLayout {
        public int Index { get; set; }
        public ISymbol TargetSymbol { get; set; }
        public ISymbol TargetType { get; set; }
        public int OutputSize { get; set; }
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
        public ICustomParserAttribute? OverrideParser { get; set; }
    }

    // Represents an analyzed type
    internal class TypeInfo {
        public INamedTypeSymbol TypeSymbol { get; }
        public List<TypeDeclarationSyntax> DeclarationSyntaxes { get; }
        public IReadOnlyList<MemberInfo> Members { get; }
        public List<AttributeData> Attributes { get; }
    }

    internal class MemberInfo {
        public int Index { get; }
        public List<MemberDeclarationSyntax> DeclarationSyntaxes { get; }
        public IReadOnlyList<AttributeData> Attributes { get; }
        public ISymbol MemberSymbol { get; }
        public INamedTypeSymbol MemberTypeSymbol { get; }
        public ICustomParserAttribute? CustomParser { get; }
        public FormatLengthInfo? LengthInfo { get; }
    }

    internal struct FormatLengthInfo {
        public int FixedLength { get; }
        public int MinLength { get; }
        public int MaxLength { get; }
        public OverflowBehaviour OverflowBehaviour { get; }
        public UnderflowBehaviour UnderflowBehaviour { get; }
    }

    public enum OverflowBehaviour {
        /// <summary>
        /// Treat the data as faulty and should stop serializing, result in a <see cref="SerializeResult.Faulted"/>
        /// </summary>
        FaultyData,

        /// <summary>
        /// Treat the data as normal, but trim excessive bytes
        /// </summary>
        TrimExcessive,
    }

    public enum UnderflowBehaviour {
        /// <summary>
        /// Treat the data as faulty and should stop serializing, result in a <see cref="SerializeResult.Faulted"/>
        /// </summary>
        FaultyData,

        /// <summary>
        /// Fill the missing data with zero bytes
        /// </summary>
        FillZero,
    }
}
