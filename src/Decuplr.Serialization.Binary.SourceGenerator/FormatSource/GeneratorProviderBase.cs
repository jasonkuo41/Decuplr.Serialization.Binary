using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;

namespace Decuplr.Serialization.Binary.FormatSource {
    internal abstract class GeneratorProviderBase : IGenerationStartup {
        
        public virtual void ConfigureFeatures(IGenerationFeatures provider) {
            provider.AddConditionResolver<>()
                    .AddConditionResolver<>();
        }

        public abstract bool TryGetSchemaInfo(NamedTypeMetaInfo metaInfo, out SchemaInfo schema);
    }
}
