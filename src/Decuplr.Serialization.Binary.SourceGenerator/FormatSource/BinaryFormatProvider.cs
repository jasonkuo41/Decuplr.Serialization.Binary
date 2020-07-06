using System;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.Binary.FormatSource {
    internal class BinaryFormatProvider : GeneratorProviderBase {

        public override SchemaPrecusor GetSchemaInfo(NamedTypeMetaInfo metaInfo) {
            var attribute = metaInfo.GetAttributeInspector<BinaryFormatAttribute>();
            if (attribute is null)
                throw new ArgumentException("Type doesn't contain [BinaryFormat] attribute");
            return new SchemaPrecusor {
                IsSealed = attribute.GetSingleValue(x => x.Sealed),
                NeverDeserialize = attribute.GetSingleValue(x => x.NeverDeserialize),
                RequestLayout = attribute.GetSingleValue(x => x.Layout),
                // Binary Format doesn't support assigning to namespaces (we could just leave this field empty, but for explicit sake)
                TargetNamespaces = new string[] { "Default" }
            };
        }

        public override bool ShouldApplyProvider(NamedTypeMetaInfo metaInfo) => metaInfo.ContainsAttribute<BinaryFormatAttribute>();

    }
}
