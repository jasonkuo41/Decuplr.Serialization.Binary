using Decuplr.Serialization.Binary;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    internal class DiagnosticHelper {
        private const string IdTitle = "BFSG";
        private const string Category = "Decuplr.BinaryPacker.SourceGen";

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
                $"Explicit layout ignores {nameof(NeverFormatAttribute)}",
                $"Explicit layout ignores {nameof(NeverFormatAttribute)} as it uses IndexAttribute to determinate what should be formatted",
                Category, DiagnosticSeverity.Warning, true);
    }
}