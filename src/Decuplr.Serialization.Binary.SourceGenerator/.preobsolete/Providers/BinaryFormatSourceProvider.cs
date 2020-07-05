using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.Schemas;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.ParserProviders {
    internal class BinaryFormatSourceProvider : IParserGenerateSource {

        public bool TryGenerateParser(IEnumerable<AnalyzedType> types, SourceGeneratorContext context, out IEnumerable<GeneratedParser>? parsers) {
            parsers = null;

            IEnumerable<AnalyzedType>? interestedType = types.Where(type => type.ContainsAttribute<BinaryFormatAttribute>());
            List<GeneratedParser>? resultParsers = new List<GeneratedParser>();
            foreach (AnalyzedType? type in interestedType) {
                if (!TryCreateParser(type, context, out GeneratedParser parser))
                    return false;
                resultParsers.Add(parser);
            }
            parsers = resultParsers;
            return true;
        }

        public bool TryCreateParser(AnalyzedType type, SourceGeneratorContext context, out GeneratedParser parser) {
            // First we check if the type's layout and dump the output of the analyzed format
            var attribute = type.GetAttributes<BinaryFormatAttribute>().FirstOrDefault();
            Debug.Assert(!attribute.IsEmpty);

            var schemaPrecusor = new SchemaPrecusor {
                IsSealed = attribute.Data.GetNamedArgumentValue<bool>(nameof(BinaryFormatAttribute.Sealed)) ?? false,
                NeverDeserialize = attribute.Data.GetNamedArgumentValue<bool>(nameof(BinaryFormatAttribute.NeverDeserialize)) ?? false,
                RequestLayout = attribute.Data.GetNamedArgumentValue<LayoutOrder>(nameof(BinaryFormatAttribute.Layout)) ?? LayoutOrder.Auto,
                // Because we aren't supporting it rn
                TargetNamespaces = new string[] { "Default" }
            };

            bool result = SchemaParserConverter.TryConvert(type, context.Compilation, schemaPrecusor, out IList<Diagnostic>? diagnostics, out parser);
            foreach (Diagnostic? diagnostic in diagnostics)
                context.ReportDiagnostic(diagnostic);
            return result;
        }
    }

}
