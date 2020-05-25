using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    static class DiagnosticHelper {
        public static DiagnosticDescriptor DuplicateIndex { get; } = new DiagnosticDescriptor("BSG-0001", "Duplicate Index", "Duplicated index of '{0}' was found in multiple occassions", "BinaryFormat.SourceGenerator", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor MissingIndex { get; } = new DiagnosticDescriptor("BSG-0002", "Missing Index", "Index '{0}' cannot be found within the type. (Did you forgot to add index attribute?)", "BinaryFormat.SourceGenerator", DiagnosticSeverity.Error, true);
    }
}
