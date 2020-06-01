using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.Analyzers {
    public class AnalyzedMember : IEquatable<AnalyzedMember> {

        private readonly SourceCodeAnalyzer Analyzer;
        private readonly List<AnalyzedPartialMember> DeclarationList = new List<AnalyzedPartialMember>();

        public bool IsPartial { get; }

        public AnalyzedPartialType ContainingType { get; }

        public AnalyzedType ContainingFullType => ContainingType.FullType;

        public ISymbol MemberSymbol { get; }

        public IReadOnlyList<AnalyzedPartialMember> Declarations => DeclarationList;

        public IEnumerable<AnalyzedAttribute> GetAttributes<T>(SymbolEqualityComparer? comparer = null) where T : Attribute {
            return Declarations.SelectMany(x => x.Attributes.SelectMany(x => x)).Where(x => x.Data.AttributeClass?.Equals(Analyzer.GetSymbol<T>(), comparer ?? SymbolEqualityComparer.Default) ?? false);
        }

        public bool ContainsAttribute<T>(SymbolEqualityComparer? comparer = null) where T : Attribute {
            return Declarations.SelectMany(x => x.Attributes.SelectMany(x => x)).Any(x => x.Data.AttributeClass?.Equals(Analyzer.GetSymbol<T>(), comparer ?? SymbolEqualityComparer.Default) ?? false);
        }

        internal AnalyzedMember(MemberDeclarationSyntax memberSyntax, SourceCodeAnalyzer analyzer, AnalyzedPartialType rootType, ISymbol symbol, SemanticModel model) {
            IsPartial = memberSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
            Analyzer = analyzer;
            ContainingType = rootType;
            MemberSymbol = symbol;
            DeclarationList.Add(new AnalyzedPartialMember(this, memberSyntax, model));
        }

        internal void AddSyntax(MemberDeclarationSyntax memberSyntax, SemanticModel model) {
            DeclarationList.Add(new AnalyzedPartialMember(this, memberSyntax, model));
        }

        public bool Equals(AnalyzedMember other) => MemberSymbol.Equals(other.MemberSymbol, SymbolEqualityComparer.Default);
        public override bool Equals(object other) => other is AnalyzedMember member && Equals(member);
        public override int GetHashCode() => MemberSymbol.GetHashCode();
    }
}
