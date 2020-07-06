using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.Binary.LayoutService;
using Decuplr.Serialization.CodeGeneration;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.Binary.FormatSource {
    internal abstract class GeneratorProviderBase : IGeneratorProvider {
        public virtual IOrderSelector OrderSelector { get; } = new IndexOrderSelector();

        public virtual void ConfigureFeatures(IFormattingFeature feature) {
            feature.AddConditionResolver<BinaryLengthResolver>();
        }

        public abstract SchemaPrecusor GetSchemaInfo(NamedTypeMetaInfo metaInfo);
        public abstract bool ShouldApplyProvider(NamedTypeMetaInfo metaInfo);
    }
}
