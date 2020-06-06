using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    static class DiagnosticHelper {
        private const string IdTitle = "BFSG";
        private const string Category = "Decuplr.BinaryPacker.SourceGenerator";

        // Move check to analyzed shared
        public static DiagnosticDescriptor ShouldDeclarePartial { get; }
            = new DiagnosticDescriptor($"{IdTitle}-001",
                "Target type has member that can only be access by partial",
                "Add `partial` keyword to type `{0}` since it's members contain format that cannot be accessed via internal or public",
                Category, DiagnosticSeverity.Error, true);
    }
}
