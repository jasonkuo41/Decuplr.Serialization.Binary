using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization;
using Decuplr.Serialization.Binary.LayoutService;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.FormatSource {
    internal class BinaryFormatProvider : GeneratorProviderBase {

        private readonly IDiagnosticReporter _reporter;

        public BinaryFormatProvider(IDiagnosticReporter reporter) {
            _reporter = reporter;
        }

        public override bool TryGetSchemaInfo(NamedTypeMetaInfo metaInfo, out SchemaInfo schema) {
            var attribute = metaInfo.GetAttributeInspector<BinaryFormatAttribute>();
            if (attribute is null) {
                schema = default;
                return false;
            }

            var layoutOrder = attribute.GetSingleValue(x => x.Layout) ?? GetConstructorOrder(attribute.Data) ?? LayoutOrder.Auto;
            var schemaBuilder = new SchemaInfoBuilder("BinaryFormat", metaInfo, new IndexOrderSelector(_reporter, layoutOrder), metaInfo.Symbol) {
                IsSealed = attribute.GetSingleValue(x => x.Sealed) ?? false,
                NeverDeserialize = attribute.GetSingleValue(x => x.NeverDeserialize) ?? false
            };
            schemaBuilder.TargetNamespaces.Add("Default");
            schema = schemaBuilder.CreateSchemaInfo();
            return true;

            static LayoutOrder? GetConstructorOrder(AttributeData data) {
                if (data.ConstructorArguments.Length == 0)
                    return null;
                return (LayoutOrder?)data.ConstructorArguments[0].Value;
            }
        }
    }
}
