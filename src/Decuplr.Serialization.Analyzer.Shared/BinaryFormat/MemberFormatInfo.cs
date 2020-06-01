using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.Binary.Analyzers;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {

    public struct BitUnion {
        public int[] Unions { get; }
    }

    public class MemberFormatInfo {
        public int Index { get; }

        public bool IsConstant { get; }

        public BitUnion? BitUnion { get; }

        public INamedTypeSymbol? FormatAs { get; }

        public string[] UsedNamespaces { get; }

        public AnalyzedMember Analyzed { get; }

        public MemberFormatInfo(AnalyzedMember member, IReadOnlyList<AnalyzedMember> referencedMember, ISet<IPropertySymbol> assiociatedSymbols) {

        }

        public static bool TryCreateFormatInfo(IReadOnlyList<AnalyzedMember> members, BinaryLayout layout, IList<Diagnostic> diagnostics, out IEnumerable<MemberFormatInfo>? formattedMembers) {
            formattedMembers = null;
            var asProp = new HashSet<IPropertySymbol>();
            var acceptedMembers = new List<AnalyzedMember>();
            for (var i = 0; i < members.Count; ++i) {
                var member = members[i];
                var result = ShouldCreateFormatInfo(member, layout, diagnostics);
                if (result < 0)
                    return false;
                if (result == 0)
                    continue;
                if (member.MemberSymbol is IFieldSymbol symbol && symbol.AssociatedSymbol is IPropertySymbol propertySymbol)
                    asProp.Add(propertySymbol);
                acceptedMembers.Add(member);
            }
            formattedMembers = acceptedMembers.Select(x => CreateFormatInfo(x, acceptedMembers, asProp)).ToList();
            return true;
        }

        // This functions decide what members we want to capture, and also tell user their stupid ideas
        // return 0 if ignored, 1 if accepted and -1 for bail out
        private static int ShouldCreateFormatInfo(AnalyzedMember member, BinaryLayout layout, List<Diagnostic> diagnostics) {
            var symbol = member.MemberSymbol;
            if (symbol.IsImplicitlyDeclared)
                return 0;
            if (!(symbol is IPropertySymbol || symbol is IFieldSymbol)) {
                if (layout == BinaryLayout.Explicit)
                    diagnostics.Add(Diagnostic.Create(DiagnosticHelper.NotPropertyOrFieldNeverFormats, member.Declarations[0].DeclaredLocation, symbol.Locations, symbol.Name));
                return 0;
            }
            if (symbol.IsStatic) {
                // Note we only mark the first appear location of declared type, we should mark the location where the [Index] is, maybe later
                if (layout == BinaryLayout.Explicit)
                    diagnostics.Add(Diagnostic.Create(DiagnosticHelper.StaticNeverFormats, member.Declarations[0].DeclaredLocation, symbol.Locations, symbol.Name));
                return 0;
            }
            if (symbol.ContainingType.TypeKind == TypeKind.Delegate) {
                if (layout == BinaryLayout.Explicit) {
                    diagnostics.Add(Diagnostic.Create(DiagnosticHelper.DelegatesNeverFormats, member.Declarations[0].DeclaredLocation, symbol.Locations, symbol.Name));
                    return -1;
                }
                diagnostics.Add(Diagnostic.Create(DiagnosticHelper.DelegatesNeverFormatsHint, member.Declarations[0].DeclaredLocation, symbol.Locations, symbol.Name));
                return 0;
            }
            return 1;
        }

        private static MemberFormatInfo CreateFormatInfo(AnalyzedMember member, IReadOnlyList<AnalyzedMember> referencedMember, ISet<IPropertySymbol> assiociatedSymbols) {

        }
    }

}
