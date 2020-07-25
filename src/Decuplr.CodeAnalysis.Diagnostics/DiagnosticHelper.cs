using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics {

    [DiagnosticSource("DSLY", "Decuplr.Serialization.LayoutService")]
    public partial class DiagnosticHelper {

        private static readonly object[] EmptyArgs = Array.Empty<object>();
        private static readonly Dictionary<string, DiagnosticDescriptor> _descriptors = new Dictionary<string, DiagnosticDescriptor>();

        public static IReadOnlyDictionary<string, DiagnosticDescriptor> Descriptors => _descriptors;

        static DiagnosticHelper() {
            var targetAsm = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetReferencedAssemblies().Contains(typeof(DiagnosticHelper).Assembly.GetName()));
            var targetClasses = targetAsm.SelectMany(x => x.GetTypes()).Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(DiagnosticSourceAttribute)));
            foreach (var targetClass in targetClasses) {
                var sourceInfo = targetClass.GetCustomAttribute<DiagnosticSourceAttribute>();
                var members = targetClass.GetMembers().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(DiagnosticAttribute)));
                foreach (var member in members) {
                    var info = member.GetCustomAttribute<DiagnosticAttribute>();
                    var descriptor = new DiagnosticDescriptor($"{sourceInfo.Prefix}{info.Id:00}", info.Title, info.Format, sourceInfo.Category, info.Severity, true);
                    _descriptors.Add(member.Name, descriptor);
                }
            }
        }

        protected static Diagnostic CreateDiagnostic(Location location, object?[]? parameters = null, [CallerMemberName] string name = "")
            => Diagnostic.Create(Descriptors[name], location, parameters ?? EmptyArgs);
        protected static Diagnostic CreateDiagnostic(Location location, IEnumerable<Location> sublocations, object?[]? parameters = null, [CallerMemberName] string name = "")
            => Diagnostic.Create(Descriptors[name], location, sublocations, parameters ?? EmptyArgs);

    }

}
