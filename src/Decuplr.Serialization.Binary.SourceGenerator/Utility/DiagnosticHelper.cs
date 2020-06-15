using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary {
    static class DiagnosticHelper {
        private const string IdTitle = "BFSG";
        private const string Category = "Decuplr.BinaryPacker.SourceGenerator";

        // Move check to analyzed shared
        public static DiagnosticDescriptor ShouldDeclarePartial { get; }
            = new DiagnosticDescriptor($"{IdTitle}001",
                "Target type has member that can only be access by partial",
                "Add `partial` keyword to type `{0}` since it's members contain format that cannot be accessed via internal or public",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor ShouldStateTypeWithGenericParser { get; }
            = new DiagnosticDescriptor($"{IdTitle}002",
                "Generic Parser Provider cannot determinate the type being parsed by usage",
                "Generic Parser Provider `{0}` cannot determinate the type being parsed by usage, state the type explicitly with `[BinaryParser]` Attribute",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor ParserProviderCountTooMuch { get; }
            = new DiagnosticDescriptor($"{IdTitle}003",
                "Generic Parser Provider or Parser Provider cannot designate more then one parsing type",
                "Generic Parser Provider / Parser Provider `{0}` cannot designate more then one parsing type at once",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor ParserProviderTypeMismatch { get; }
            = new DiagnosticDescriptor($"{IdTitle}004",
                "Conflict parsing type between IParserProvider / TypeParser and attribute [BinaryParser]",
                "Expected `{0}` as IParserProvider / TypeParser stated, but type `{1}` was designated via attribute. Explicit type designation is not needed.",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor ParserProviderImplicitReport { get; }
            = new DiagnosticDescriptor($"{IdTitle}005",
                "IParserProvider can implicity state the type being parsed",
                "IParserProvider can implicity state the type `{0}` being parsed, explicitly stating in attribute [BinaryParser] is not needed.",
                Category, DiagnosticSeverity.Info, true);

        public static DiagnosticDescriptor TypeParserRequiresDefaultConstructor { get; }
            = new DiagnosticDescriptor($"{IdTitle}006",
                "TypeParser requires default constructor for auto registration",
                "TypeParser `{0}` requires a default constructor if it's using [BinaryParser] to automatically register parser.",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor ExplicitTypeMushHaveConstructor { get; }
            = new DiagnosticDescriptor($"{IdTitle}007",
                "BinaryParser explicit specifies a type that does not have corresponding constructor and destructor pair",
                "BinaryParser explicit specifies `{0}` to be resolved, but does not have a corresponding consturctor taking one agrument of type `{0}` or destructor (ITypeConvertible or ConvertTo)",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor ConvertToIsIgnoredIfHasInterface { get; }
            = new DiagnosticDescriptor($"{IdTitle}008",
                "Deconstruction method is ignored, due to interface method present",
                "Deconstruction method `{0}` is ignored because this type implementes ITypeConvertible which has higher priority",
                Category, DiagnosticSeverity.Warning, true);

        public static DiagnosticDescriptor DeconstructIsIgnoredDueToNoConstructor { get; }
            = new DiagnosticDescriptor($"{IdTitle}009",
                "Deconstruction method is ignored, because no corresponding constructor was found",
                "Deconstruction method for `{0}` is ignored because no corresponding constructor that takes 1 argument that is `{0}` type was found",
                Category, DiagnosticSeverity.Warning, true);

        public static DiagnosticDescriptor CannotFindMatchingParserType { get; }
            = new DiagnosticDescriptor($"{IdTitle}010",
                "Cannot find any parsing target in this type",
                "Unable to locate any constructor and deconstructor in this type to identify any parsing target",
                Category, DiagnosticSeverity.Error, true);
    }
}
