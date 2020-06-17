using Decuplr.Serialization.Binary;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    internal class DiagnosticHelper {
        private const string IdTitle = "BPSA"; // BinaryPacker Shared Analyzer
        private const string Category = "Decuplr.BinaryPacker.Analyzer";

        public static DiagnosticDescriptor SequentialShouldNotIndex { get; }
            = new DiagnosticDescriptor($"{IdTitle}-001",
                "Sequential layout shouldn't include any IndexAttribute",
                "When BinaryFormat is marked as BinaryLayout.Sequential, it shouldn't include any IndexAttribute",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor AutoAsSequentialTooMuchDeclare { get; }
            = new DiagnosticDescriptor($"{IdTitle}-002",
                "Auto layout (assume as sequential layout with no [Index] attribute present in type) shouldn't contain multiple declaration",
                "There can be no defined ordering between properties or fields in mutiple declaration of partial type {0}." +
                "To specify an ordering, either make all instances within the same declaration or mark [Index] attribute to define order",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor SequentialTooMuchDeclare { get; }
            = new DiagnosticDescriptor($"{IdTitle}-003",
                "Sequetial layout shouldn't contain mutliple declaration",
                "There can be no defined ordering between properties or field in mutiple declaration of partial type {0}." +
                "To specify an ordering, either make all instances within the same declaration or use 'BinaryLayout.Explicit' layout",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor ExplicitNoIndex { get; }
            = new DiagnosticDescriptor($"{IdTitle}-004",
                "Explicit layout expects [Index] attribute",
                "Explicit layout requires [Index] attribute to indentify what should be formatted in what order",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor ExplicitDontNeedNeverFormat { get; }
            = new DiagnosticDescriptor($"{IdTitle}-005",
                $"Explicit layout ignores {nameof(IgnoreAttribute)}",
                $"Explicit layout ignores {nameof(IgnoreAttribute)} as it uses IndexAttribute to determinate what should be formatted",
                Category, DiagnosticSeverity.Warning, true);

        public static DiagnosticDescriptor StaticNeverFormats { get; }
            = new DiagnosticDescriptor($"{IdTitle}-006",
                "Static type members will never be serialized",
                "Static type member `{0}` will never be serialized, and should not be marked with any IndexAttribute and alike",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor ConstNeverFormats { get; }
            = new DiagnosticDescriptor($"{IdTitle}-007",
                "Const members will never be serialized",
                "Const member `{0}` will never be serialized, and should not be marked with any IndexAttribute and alike",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor NotPropertyOrFieldNeverFormats { get; }
            = new DiagnosticDescriptor($"{IdTitle}-008",
                "Type members that are not property or fields will never be serialized",
                "Type members `{0}` will never be serialized as it is not property or field",
                Category, DiagnosticSeverity.Warning, true);

        public static DiagnosticDescriptor DelegatesNeverFormats { get; }
            = new DiagnosticDescriptor($"{IdTitle}-009",
                "Property or field returning delegates will not be correctly serialized",
                "Property or field `{0}` returning delegates is not supported for serializing or formatting",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor DelegatesNeverFormatsHint { get; }
            = new DiagnosticDescriptor($"{IdTitle}-010",
                "Property or field returning delegates will not be correctly serialized",
               $"Property or field `{{0}}` returning delegates is not supported for serializing or formatting, consider adding {nameof(IgnoreAttribute)} for clearity",
                Category, DiagnosticSeverity.Info, true);

        public static DiagnosticDescriptor DuplicateIndexs { get; }
            = new DiagnosticDescriptor($"{IdTitle}-011",
                "Found duplicate indexs",
                "Found duplicate index `{0}`, which is not allowed.",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor ShouldApplyConstant { get; }
            = new DiagnosticDescriptor($"{IdTitle}-012",
                "Property or field that cannot be modified during runtime should be marked with constant",
                "Property or field `{0}` cannot be modified during runtime. It should be marked with constant to explicitly imply it's behaviour, if it should not be serialized, then it should be marked with [Ignore]",
                Category, DiagnosticSeverity.Warning, true);

        public static DiagnosticDescriptor PropertyCannotBeWriteOnly { get; }
            = new DiagnosticDescriptor($"{IdTitle}-013",
                "Property cannot be write only",
                "Property `{0}` cannot be write only, it must contain get methods",
                Category, DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor CannotApplyNamespacePartial { get; }
            = new DiagnosticDescriptor($"{IdTitle}-014",
                "Cannot apply namespaces across different declaring location",
                "Cannot apply namespace `{0}` because other namespace apply were declared else where, making it impossible to determinate order",
                Category, DiagnosticSeverity.Error, true);
    }
}