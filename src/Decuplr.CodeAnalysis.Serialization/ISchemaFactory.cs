using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface ISchemaFactory {
        /// <summary>
        /// The layout of the schema
        /// </summary>
        SchemaLayout Layout { get; }

        /// <summary>
        /// Generate the target schema
        /// </summary>
        /// <param name="uniqueName">A unique name for the parser</param>
        /// <param name="type">The target type, must be included in the <see cref="SchemaInfo.TargetTypes"/> </param>
        /// <param name="componentProvider">The provider that generates the component needed for composing the type</param>
        GeneratedParserInfo ComposeSchema(string uniqueName, INamedTypeSymbol type, IComponentProvider componentProvider);
    }
}
