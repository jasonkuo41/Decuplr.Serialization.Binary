using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.SourceGenerator {

    internal struct AnalyzedAttribute {
        public AttributeData Data { get; set; }
        public Location Location { get; set; }

        public static implicit operator AttributeData(AnalyzedAttribute attribute) => attribute.Data;
    }

    internal class AnalyzedPartialType {

        public AnalyzedType ContainingType { get; }

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
            ContainingType = type;
            DeclaredLocation = syntax.GetLocation();
            Members = syntax.Members.Select(memberSyntax => new AnalyzedMember(memberSyntax)).ToList();

            var typeAttributes = ContainingType.TypeSymbol.GetAttributes();
            Attributes = syntax.AttributeLists.Select(list => {
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

    // Represents an analyzed type
    internal class AnalyzedType {

        private readonly SourceGeneratorContext _context;
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
        /// All the declartions found for this type, would be more then one if partial
        /// </summary>
        public IReadOnlyList<AnalyzedPartialType> Declarations => _declartions;

        /// <summary>
        /// Adds existing declaration syntax to the info
        /// </summary>
        public void AddSyntax(TypeDeclarationSyntax typeSyntax) {
            var model = _context.Compilation.GetSemanticModel(typeSyntax.SyntaxTree, true);
            if (!(model.GetDeclaredSymbol(typeSyntax, _context.CancellationToken)?.Equals(TypeSymbol, SymbolEqualityComparer.Default) ?? false))
                throw new ArgumentException($"Syntax is not type of {TypeSymbol}");
            _declartions.Add(new AnalyzedPartialType(this, typeSyntax, model));
        }

        public AnalyzedType(TypeDeclarationSyntax declarationSyntax, SourceGeneratorContext context) {
            IsPartial = declarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
            _context = context;
            var semanticModel = context.Compilation.GetSemanticModel(declarationSyntax.SyntaxTree, true);
            TypeSymbol = semanticModel.GetDeclaredSymbol(declarationSyntax, context.CancellationToken)!;

            _declartions.Add(new AnalyzedPartialType(this, declarationSyntax, semanticModel));
        }
    }

    internal class AnalyzedPartialMember {

    }

    internal class AnalyzedMember {

        public bool IsPartial { get; }

        public ISymbol MemberSymbol { get; }

        public IReadOnlyList<IReadOnlyList<IReadOnlyList<AttributeData>>> Attributes { get; }

        // IsWriteable

        public AnalyzedMember(MemberDeclarationSyntax memberSyntax) {

        }

    }
}
