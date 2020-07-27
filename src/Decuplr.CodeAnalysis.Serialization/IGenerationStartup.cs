using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.CodeAnalysis.Serialization {
    /// <summary>
    /// A startup entry point for code generation
    /// </summary>
    /// <remarks>
    /// Dependency Injected services are limited at this scope
    /// </remarks>
    public interface IGenerationStartup {
        string Name { get; }
        void ConfigureFeatures(IGenerationFeatures provider);
        bool TryGetSchemaInfo(NamedTypeMetaInfo metaInfo, out SchemaInfo schema);
    }
}
