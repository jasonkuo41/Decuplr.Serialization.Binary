using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService.Internal {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class DiagnosticAttribute : Attribute {
        public DiagnosticAttribute(int id, DiagnosticSeverity severity, string title, string format) {
            Id = id;
            Severity = severity;
            Title = title;
            Format = format;
        }

        public int Id { get; }
        public DiagnosticSeverity Severity { get; }
        public string Title { get; }
        public string Format { get; }
    }
}
