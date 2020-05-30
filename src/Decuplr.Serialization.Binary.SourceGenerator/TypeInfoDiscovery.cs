using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    static class TypeInfoDiscovery {
        public static bool TryParseType(INamedTypeSymbol typeSymbol, SourceGeneratorContext context, out AnalyzedType? format) {
            format = null;
            var compilation = context.Compilation;
            var indexSymbol = compilation.GetTypeByMetadataName(typeof(IndexAttribute).FullName);
            var candidateMembers = typeSymbol.GetMembers()
                // member must either be field or property
                .Where(member => member is IFieldSymbol || member is IPropertySymbol)
                .Where(member => member.GetAttributes().Any(x => x.AttributeClass?.Equals(indexSymbol, SymbolEqualityComparer.Default) ?? false))
                .ToList();

            var lookup = new Dictionary<int, MemberFormatInfo>();
            // Add these member to a dictionary
            foreach (var member in candidateMembers) {
                // There can only be one [Index] attribute
                var indexAttribute = member.GetAttributes().First(x => x.AttributeClass?.Equals(indexSymbol, SymbolEqualityComparer.Default) ?? false);
                // We know that the constructor with index must be IndexAt
                var indexAt = (int)indexAttribute.ConstructorArguments[0].Value!;
                // Detect if duplicate index exists
                if (lookup.ContainsKey(indexAt)) {
                    // TODO : Dump better location, note this only dumps at it's first declared location (which will miss if it's declared partial (but who does that?!))
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.DuplicateIndex, member.Locations.First(), indexAt));
                    return false;
                }

                lookup.Add(indexAt, new MemberFormatInfo(indexAt, member));

            }

            // Check if lookup is correctly filled
            // I know there's a better way to do this, just too lazy to figure it out :(
            var layoutlist = lookup.OrderBy(x => x.Key).Select(x => x.Value).ToList();
            for (var i = 0; i < layoutlist.Count; ++i) {
                if (layoutlist[i].Index == i)
                    continue;
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.MissingIndex, typeSymbol.Locations.First(), i));
                return false;
            }
            format = new AnalyzedType(typeSymbol, layoutlist);
            return true;
        }
    }
}
