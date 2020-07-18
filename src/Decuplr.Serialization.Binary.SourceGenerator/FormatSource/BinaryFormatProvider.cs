using System;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.Binary.FormatSource {
    internal class BinaryFormatProvider : GeneratorProviderBase {

        public override bool TryGetSchemaInfo(NamedTypeMetaInfo metaInfo, out SchemaConfig schema) {
            var attribute = metaInfo.GetAttributeInspector<BinaryFormatAttribute>();
            if (attribute is null) {
                schema = default;
                return false;
            }
            schema = new SchemaConfig {
                IsSealed = attribute.GetSingleValue(x => x.Sealed),
                NeverDeserialize = attribute.GetSingleValue(x => x.NeverDeserialize),
                RequestLayout = attribute.GetSingleValue(x => x.Layout),
                // Binary Format doesn't support assigning to namespaces (we could just leave this field empty, but for explicit sake)
                TargetNamespaces = new string[] { "Default" }
            };
            return true;
        }
    }
}
