using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IGenerationSource {
        IOrderSelector OrderSelector { get; }
        void ConfigureFeatures(IGenerationFeatures provider);
        bool TryGetSchemaInfo(NamedTypeMetaInfo metaInfo, out SchemaPrecusor schema);
    }
}
