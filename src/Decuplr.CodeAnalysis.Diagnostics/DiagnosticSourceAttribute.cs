using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.CodeAnalysis.Diagnostics {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DiagnosticSourceAttribute : Attribute {
        public DiagnosticSourceAttribute(string prefix, string category) {
            Prefix = prefix;
            Category = category;
        }

        public string Prefix { get; }
        public string Category { get; }
    }
}
