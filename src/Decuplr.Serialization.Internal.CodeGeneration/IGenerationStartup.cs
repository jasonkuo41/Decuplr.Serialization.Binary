using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    /// <summary>
    /// A startup entry point for code generation
    /// </summary>
    /// <remarks>
    /// Dependency Injected services are limited at this scope
    /// </remarks>
    public interface IGenerationStartup {
        void ConfigureFeatures(IGenerationFeatures provider);
        bool TryGetSchemaInfo(NamedTypeMetaInfo metaInfo, out SchemaInfo schema, out IOrderSelector orderSelector);
    }
}
