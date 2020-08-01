using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.Serialization.Binary.LayoutService;

namespace Decuplr.Serialization.Binary.FormatSource {
    internal abstract class GeneratorProviderBase : IGenerationStartup {
        public virtual IOrderSelector OrderSelector { get; } = new IndexOrderSelector();

        public virtual void ConfigureFeatures(IGenerationFeatures feature) {
            feature.AddConditionResolver<BinaryLengthResolver>();

        }

        public abstract bool TryGetSchemaInfo(NamedTypeMetaInfo metaInfo, out SchemaConfig schema);
    }
}
