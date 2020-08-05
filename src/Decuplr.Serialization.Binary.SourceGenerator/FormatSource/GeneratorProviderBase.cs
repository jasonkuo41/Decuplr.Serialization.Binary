using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.Serialization.Binary.ConditionResolver;

namespace Decuplr.Serialization.Binary.FormatSource {
    internal abstract class GeneratorProviderBase : IGenerationStartup {
        
        public virtual void ConfigureFeatures(IGenerationFeatures provider) {
            provider.AddConditionResolver<IgnoreIfConditionProvider>();
        }

        public abstract bool TryGetSchemaInfo(NamedTypeMetaInfo metaInfo, out SchemaInfo schema);
    }
}
