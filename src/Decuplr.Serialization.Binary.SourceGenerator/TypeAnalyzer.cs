using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.SourceGenerator {

    public struct GeneratedSourceCode {
        public string DesiredFileName { get; set; }
        public string SourceText { get; set; }
    }

    public struct FormatterSourceCode {
        public string[] UsingStatements { get; set; }
        public string SourceText { get; set; }
    }

    internal class TypeAnalyzer {
        public INamedTypeSymbol TypeSymbol { get; }
        public TypeDeclarationSyntax Syntax { get; }
        public IReadOnlyList<TypeLayout> Layouts { get; }
        public string GeneratedSerializerFullName { get; }

        public string FormatterSourceCode { get; }
        public IReadOnlyList<GeneratedSourceCode> GeneratedSourceCodes { get; }

        public TypeAnalyzer(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol) {
            TypeSymbol = symbol;
            Syntax = syntax;
        }

        private static TypeAnalyzer UsePartialStrategy(SourceGeneratorContext context, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, IReadOnlyList<TypeLayout> layouts) {
            var partialClassBuilder = new PartialClassBuilder(symbol, layouts);
            // Verifies if it's applicable for such behaviour
            partialClassBuilder.Verify();
            partialClassBuilder.CreatePartialClass();
            FormatterSourceCode = partialClassBuilder.CreateFormatter();
        }

        private static TypeAnalyzer UseSimpleFormatterStrategy() {

        }

        public static TypeAnalyzer? ParseType(SourceGeneratorContext context, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol) {
            var compilation = context.Compilation;
            var indexSymbol = compilation.GetTypeByMetadataName(typeof(IndexAttribute).FullName);
            var candidateMembers = symbol.GetMembers()
                // member must either be field or property
                .Where(member => member is IFieldSymbol || member is IPropertySymbol)
                .Where(member => member.GetAttributes().Any(x => x.AttributeClass.Equals(indexSymbol, SymbolEqualityComparer.Default)))
                .ToList();

            var lookup = new Dictionary<int, TypeLayout>();
            // Add these member to a dictionary
            foreach(var member in candidateMembers) {
                var indexAttribute = member.GetAttributes().First(x => x.AttributeClass.Equals(indexSymbol, SymbolEqualityComparer.Default));

                var indexAt = indexAttribute.GetArgument<int>(nameof(IndexAttribute.Index));
                // Detect if duplicate index exists
                if (lookup.ContainsKey(indexAt)) {
                    // TODO : Dump location, note this only dumps at it's first location (which is not that desired, need to improve)

                    return null;
                }

                lookup.Add(indexAt, new TypeLayout { 
                    Index = indexAt,
                    TargetSymbol = member,
                    TargetType = (member as IParameterSymbol)?.Type ?? (member as IFieldSymbol)?.Type,
                    // TODO : Expand Type Layout
                });

            }

            // Check if lookup is correctly filled
            // I know there's a better way to do this, just too lazy to figure it out :(
            var layoutlist = lookup.OrderBy(x => x.Key).Select(x => x.Value).ToList();
            for(var i = 0; i < layoutlist.Count; ++i) {
                if (layoutlist[i].Index == i)
                    continue;
                // TODO : Dump location, it's not correctly filled!

                return null;
            }

            // We are done here
            // Check what strategy we are going with : partial class or simple formatter
            // TODO : We can have stragety load into here to find the best, but that's another day business

            // If we can get all symbols correctly, then congrats, simple formatter!
            // We can even go as far as using MemoryMarshal for simple struct types, but we'll do it other day
            if (candidateMembers.All(x => x.CanAccessSymbolInternally()))
                return UseSimpleFormatterStrategy();
            return UsePartialStrategy(context, syntax, symbol, layoutlist);
        }

        public void AddToContextSource(SourceGeneratorContext context) { }

        public override string ToString() => TypeSymbol.GetClassFullName();
    }
}
