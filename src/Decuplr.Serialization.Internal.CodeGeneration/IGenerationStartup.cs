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
        IOrderSelector OrderSelector { get; }
        void ConfigureFeatures(IGenerationFeatures provider);
        bool TryGetSchemaInfo(NamedTypeMetaInfo metaInfo, out SchemaConfig schema);
    }
}
