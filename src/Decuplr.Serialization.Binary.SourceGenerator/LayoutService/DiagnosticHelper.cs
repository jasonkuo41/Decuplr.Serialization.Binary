using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Decuplr.Serialization.Binary.AnalysisService;
using Decuplr.Serialization.Binary.LayoutService.Internal;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService.Internal {
}

namespace Decuplr.Serialization.Binary.LayoutService {
    internal partial class DiagnosticHelper {

        private const string Prefix = "BSL";
        private const string Category = "Decuplr.Serialization.Binary";
        private static readonly object[] EmptyArgs = Array.Empty<object>();

        public static Dictionary<string, DiagnosticDescriptor> Descriptors { get; } = new Dictionary<string, DiagnosticDescriptor>();

        static DiagnosticHelper() {
            var members = typeof(Diagnostic).GetMembers().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(DiagnosticAttribute)));
            foreach (var member in members) {
                var info = member.GetCustomAttribute<DiagnosticAttribute>();
                Descriptors.Add(member.Name, CreateDescriptor(info.Id, info.Title, info.Format, info.Severity));
            }
        }

        private static DiagnosticDescriptor CreateDescriptor(int id, string title, string format, DiagnosticSeverity severity)
            => new DiagnosticDescriptor($"{Prefix}{id:000}", title, format, Category, severity, true);
        private static Diagnostic CreateDiagnostic(Location location, object?[]? parameters = null, [CallerMemberName] string name = "")
            => Diagnostic.Create(Descriptors[name], location, parameters ?? EmptyArgs);
        private static Diagnostic CreateDiagnostic(Location location, IEnumerable<Location> sublocations, object?[]? parameters = null, [CallerMemberName] string name = "")
            => Diagnostic.Create(Descriptors[name], location, sublocations, parameters ?? EmptyArgs);


    }

}
