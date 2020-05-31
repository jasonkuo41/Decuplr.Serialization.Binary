using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.Analyzers {
    public class AnalyzedPartialMember {

        public AnalyzedMember ContainingMember { get; }

        public Location DeclaredLocation { get; }

        /// <summary>
        /// The outer list indicates it's vertical order([A] [B]), while inner list indicates it's horizontal order ([A, B])
        /// </summary>
        public IReadOnlyList<IReadOnlyList<AnalyzedAttribute>> Attributes { get; }

        public AnalyzedPartialMember(AnalyzedMember member, MemberDeclarationSyntax declarationSyntax, SemanticModel model) {
            ContainingMember = member;
            DeclaredLocation = declarationSyntax.GetLocation();
            Attributes = declarationSyntax.GetAttributeDataFrom(member.MemberSymbol);
        }

    }
}
