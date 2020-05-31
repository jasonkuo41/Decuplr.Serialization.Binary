using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.SourceGenerator {

    internal static class AnalyzedUtilities {
        public static IReadOnlyList<IReadOnlyList<AnalyzedAttribute>> GetAttributeDataFrom(this MemberDeclarationSyntax syntax, ISymbol sourceSymbol) {
            var typeAttributes = sourceSymbol.GetAttributes();
            return syntax.AttributeLists.Select(list => {
                return list.Attributes.Select(ho => {
                    return new AnalyzedAttribute {
                        // This get's the AttributeData from the type
                        // TODO : Please make sure this works!
                        Data = typeAttributes.Where(x => ho.GetReference().Equals(x.ApplicationSyntaxReference)).First(),
                        Location = ho.GetLocation()
                    };
                }).ToList();
            }).ToList();
        }
    }

    internal struct AnalyzedAttribute {
        public AttributeData Data { get; set; }
        public Location Location { get; set; }

        public static implicit operator AttributeData(AnalyzedAttribute attribute) => attribute.Data;
    }

    internal class AnalyzedPartialType {

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

        public AnalyzedPartialType(AnalyzedType type, TypeDeclarationSyntax syntax, SemanticModel model) {
            FullType = type;
            DeclaredLocation = syntax.GetLocation();

            var memberDictionary = new Dictionary<ISymbol, AnalyzedMember>();
            var members = new List<AnalyzedMember>();
            foreach(var member in syntax.Members) {
                // ct?
                var memberSymbol = model.GetDeclaredSymbol(member)!;
                if (memberDictionary.TryGetValue(memberSymbol, out var analyzedMember)) {
                    analyzedMember.AddSyntax(member, model);
                    continue;
                }
                analyzedMember = new AnalyzedMember(member, this, memberSymbol, model);
                members.Add(analyzedMember);
                memberDictionary.Add(memberSymbol, analyzedMember);
            }
            Members = members;
            Attributes = syntax.GetAttributeDataFrom(type.TypeSymbol);
        }
    }

    // Represents an analyzed type
    internal class AnalyzedType {

        private readonly Compilation Compilation;
        private readonly CancellationToken CancellationToken;
        private readonly List<AnalyzedPartialType> _declartions = new List<AnalyzedPartialType>();

        /// <summary>
        /// If the class is partial or not
        /// </summary>
        public bool IsPartial { get; }
        
        /// <summary>
        /// The Symbol that represents the type
        /// </summary>
        public INamedTypeSymbol TypeSymbol { get; }

        /// <summary>
        /// All the declarations found for this type, would be more then one if partial
        /// </summary>
        public IReadOnlyList<AnalyzedPartialType> Declarations => _declartions;

        /// <summary>
        /// Adds existing declaration syntax to the info
        /// </summary>
        public void AddSyntax(TypeDeclarationSyntax typeSyntax) {
            var model = Compilation.GetSemanticModel(typeSyntax.SyntaxTree, true);
            if (!(model.GetDeclaredSymbol(typeSyntax, CancellationToken)?.Equals(TypeSymbol, SymbolEqualityComparer.Default) ?? false))
                throw new ArgumentException($"Syntax is not type of {TypeSymbol}");
            _declartions.Add(new AnalyzedPartialType(this, typeSyntax, model));
        }

        public AnalyzedType(TypeDeclarationSyntax declarationSyntax, Compilation compilation, CancellationToken cancellationToken) {
            IsPartial = declarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
            Compilation = compilation;
            CancellationToken = cancellationToken;

            var semanticModel = Compilation.GetSemanticModel(declarationSyntax.SyntaxTree, true);
            TypeSymbol = semanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken)!;

            _declartions.Add(new AnalyzedPartialType(this, declarationSyntax, semanticModel));
        }
    }

    internal class AnalyzedPartialMember {

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

    internal class AnalyzedMember : IEquatable<AnalyzedMember> {

        private readonly List<AnalyzedPartialMember> DeclarationList = new List<AnalyzedPartialMember>();

        public bool IsPartial { get; }

        public AnalyzedPartialType ContainingType { get; }

        public AnalyzedType ContainingFullType => ContainingType.FullType;

        public ISymbol MemberSymbol { get; }

        public IReadOnlyList<AnalyzedPartialMember> Declarations => DeclarationList;

        internal AnalyzedMember(MemberDeclarationSyntax memberSyntax, AnalyzedPartialType rootType, ISymbol symbol, SemanticModel model) {
            IsPartial = memberSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
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
