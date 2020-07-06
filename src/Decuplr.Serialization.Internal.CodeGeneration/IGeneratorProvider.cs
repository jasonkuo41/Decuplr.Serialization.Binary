using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IGeneratorProvider {
        IOrderSelector OrderSelector { get; }
        void ConfigureFeatures(IFormattingFeature provider);
        SchemaPrecusor GetSchemaInfo(NamedTypeMetaInfo metaInfo);
        bool ShouldApplyProvider(NamedTypeMetaInfo metaInfo);
    }
}
