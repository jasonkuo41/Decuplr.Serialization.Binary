using System.Collections.Generic;
using Decuplr.Serialization.Binary.Analyzers;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    public class MemberFormatInfo {
        public int Index { get; }

        public int IsBitUnion { get; }

        public ISymbol? FormatAs { get; }

        public string[] UsedNamespaces { get; }

        protected MemberFormatInfo(AnalyzedMember member, int index) {

        }

        public static bool TryCreateFormatInfo(IReadOnlyList<AnalyzedMember> members, List<Diagnostic> diagnostics, out IEnumerable<MemberFormatInfo> formattedMembers) {

        }

    }

}
