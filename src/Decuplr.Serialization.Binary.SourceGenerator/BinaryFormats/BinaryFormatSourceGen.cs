using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.SourceGenerator.Solutions;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator.BinaryFormatSource {
    internal class BinaryFormatSourceGen : IParserGenerateSource {

        public bool TryGenerateParser(IEnumerable<AnalyzedType> types, SourceGeneratorContext context, out IEnumerable<GeneratedParser>? parsers) {
            parsers = null;

            var interestedType = types.Where(type => type.ContainsAttribute<BinaryFormatAttribute>());
            var resultParsers = new List<GeneratedParser>();
            foreach(var type in interestedType) {
                if (!TryCreateParser(type, context, out var parser))
                    return false;
                resultParsers.Add(parser);
            }
            parsers = resultParsers;
            return true;
        }

        public bool TryCreateParser(AnalyzedType type, SourceGeneratorContext context, out GeneratedParser parser) {
            // First we check if the type's layout and dump the output of the analyzed format
            parser = default;
            var attribute = type.GetAttributes<BinaryFormatAttribute>().FirstOrDefault();
            if (attribute.IsEmpty)
                return false;
            var formatInfo = new FormatInfo {
                IsSealed = attribute.Data.GetNamedArgumentValue<bool>(nameof(BinaryFormatAttribute.Sealed)) ?? false,
                NeverDeserialize = attribute.Data.GetNamedArgumentValue<bool>(nameof(BinaryFormatAttribute.NeverDeserialize)) ?? false,
                RequestLayout = attribute.Data.GetNamedArgumentValue<BinaryLayout>(nameof(BinaryFormatAttribute.Layout)) ?? BinaryLayout.Auto,
                // Because we aren't supporting it rn
                TargetNamespaces = Array.Empty<string>()
            };
            var result = TypeFormatLayout.TryGetLayout(type, ref formatInfo, out var diagnostics, out var typeLayout);
            foreach (var diagnostic in diagnostics)
                context.ReportDiagnostic(diagnostic);
            if (!result)
                return false;

            // Check if the target parser would need to use `partial` keyword to help up invoke into the code
            // I mean if it's already partial, we might as well as embed into it

            /* Currently, there are since it's kind of complicated to decide whether we can not use partial, so we now force users to partial everything  */
            // If it's not partial, we check if we can access every member with 
            //if (CanAccessAllGetValue(typeLayout) && (typeLayout.CanDeserialize || CanAccessAllSetValue(typeLayout))) {
                // If we can then we use observer class to invoke the class
            
            //}
            // Otherwise the type must be partial, or else we don't like it
            if (!typeLayout!.Type.IsPartial) {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticHelper.ShouldDeclarePartial, typeLayout.Type.Declarations[0].DeclaredLocation, typeLayout.Type.TypeSymbol));
                return false;
            }
            // We pump a constructor for our sweet partial class here
            parser = new TypeParserGenerator(typeLayout, new PartialTypeDeserialize(context.Compilation, typeLayout), new PartialTypeSerialize(context.Compilation, typeLayout)).GetFormatterCode();
            return true;
        }
    }

}
