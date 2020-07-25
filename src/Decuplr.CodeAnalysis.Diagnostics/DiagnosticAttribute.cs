using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DiagnosticAttribute : Attribute {
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
