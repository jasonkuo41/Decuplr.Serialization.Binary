using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface ISchemaFactory {
        /// <summary>
        /// The layout of the schema
        /// </summary>
        SchemaLayout Layout { get; }

        /// <summary>
        /// The info regard of this schema
        /// </summary>
        SchemaInfo Info { get; }

        /// <summary>
        /// Generate the target schema
        /// </summary>
        /// <param name="parserName">The name of the generated schema</param>
        /// <param name="type">The target type, must be included in the <see cref="SchemaInfo.TargetTypes"/> </param>
        /// <param name="componentProvider">The provider that generates the component needed for composing the type</param>
        GeneratedParserInfo ComposeSchema(string parserName, INamedTypeSymbol type, IComponentProvider componentProvider);
    }
}
