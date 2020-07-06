using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IGeneratorProvider {
        IOrderSelector OrderSelector { get; }
        void ConfigureFeatures(IFormattingFeature provider);
        bool TryGetSchemaInfo(NamedTypeMetaInfo metaInfo, out SchemaPrecusor schema);
    }
}
