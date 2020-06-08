using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.Analyzers {
    public class AnalyzedPartialType {

        /// <summary>
        /// The root type for this partial type
        /// </summary>
        public AnalyzedType FullType { get; }

        /// <summary>
        /// The location of the type declaration
        /// </summary>
        public Location DeclaredLocation { get; }

        /// <summary>
        /// The outer list indicates it's vertical order([A] [B]), while inner list indicates it's horizontal order ([A, B])
        /// </summary>
        public IReadOnlyList<IReadOnlyList<AnalyzedAttribute>> Attributes { get; }

        /// <summary>
        /// The members of the partial type
        /// </summary>
        public IReadOnlyList<AnalyzedMember> Members { get; }

        public AnalyzedPartialType(AnalyzedType type, TypeDeclarationSyntax syntax, SourceCodeAnalyzer analyzer, SemanticModel model) {
            FullType = type;
            DeclaredLocation = syntax.GetLocation();

            var memberDictionary = new Dictionary<ISymbol, AnalyzedMember>();
            var members = new List<AnalyzedMember>();
            foreach(var member in syntax.Members) {
                // ct?
                var memberSymbol = model.GetDeclaredSymbol(member)!;
                if (memberSymbol is null)
                    continue;
                if (memberDictionary.TryGetValue(memberSymbol, out var analyzedMember)) {
                    analyzedMember.AddSyntax(member, model);
                    continue;
                }
                analyzedMember = new AnalyzedMember(member, analyzer, this, memberSymbol, model);
                members.Add(analyzedMember);
                memberDictionary.Add(memberSymbol, analyzedMember);
            }
            Members = members;
            Attributes = syntax.GetAttributeDataFrom(type.TypeSymbol);
        }
    }
}
