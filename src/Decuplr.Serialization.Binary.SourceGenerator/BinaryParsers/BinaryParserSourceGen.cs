using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.Annotations;
using Decuplr.Serialization.Binary.Annotations.Namespaces;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator.BinaryParsers {

    /*   [BinaryParser] comes with two flavor
     *   1. Native Type : Inherits TypeParser, these are natively consider "sealed" as they cannot be modified, unless it accepts `IParserDiscovery`
     *   2. Interpret Type : Accepts a certain type as constructor, and has ConvertTo() method that returns the same type, these are not sealed unless specified
     *
     */

    internal class BinaryParserSourceGen : IParserGenerateSource {
        public bool TryGenerateParser(IEnumerable<AnalyzedType> types, SourceGeneratorContext context, out IEnumerable<GeneratedParser> parsers) {
            parsers = Array.Empty<GeneratedParser>();
            var interestedType = types.Where(type => type.ContainsAttribute<BinaryParserAttribute>());
            var resultParsers = new List<GeneratedParser>();
            foreach(var type in interestedType) {
                // Since it's inherited from type parser it's a native type
                if (type.InheritFrom<TypeParser>()) {
                    if (!TryGetNativeTypeParser(type, context, out var parser))
                        return false;
                    resultParsers.Add(parser);
                }
                var attribute = type.GetAttributes<BinaryParserAttribute>().FirstOrDefault();
                // If native type and does not accept IParserDiscovery as the only parameter in the constructor
                // We also need type parser to either have no constructor at all, or one parameter constructor with IParserDiscovery
                "Check constructor parameter for parser";
                //
                var isNativeAndSealed = type.InheritFrom<TypeParser>() && !type.TypeSymbol.Constructors.Where(x => x.Parameters.Length == 1).Any(x => x.Parameters[0].Type.Equals(type.Analyzer.GetSymbol<IParserDiscovery>(), SymbolEqualityComparer.Default));
                var formatInfo = new FormatInfo {
                    // Native type 
                    IsSealed = isNativeAndSealed || (attribute.Data.GetNamedArgumentValue<bool>(nameof(BinaryParserAttribute.Sealed)) ?? false),
                    RequestLayout = attribute.Data.GetNamedArgumentValue<BinaryLayout>(nameof(BinaryParserAttribute.Layout)) ?? BinaryLayout.Auto,
                    // We don't provide, "please don't deserialize option", you throw or deny deserialization at that level
                    NeverDeserialize = false,
                    // BinaryParser's namespace is provided by `ParserNamespace`
                    TargetNamespaces = type.GetAttributes<BinaryParserNamespaceAttribute>().Select(x => (x.Data.ConstructorArguments[0].Value as string)!).ToArray()
                };

            }
            parsers = resultParsers;
            return true;
        }

        // Slightly native
        // And there's "absolute native" that just inherits IParserProvider
        private bool TryGetNativeTypeParser(AnalyzedType type, SourceGeneratorContext context, out GeneratedParser parser) {
            parser = default;
            // Native Type can either have default constructor or have one constructor that accepts IParserDiscovery, otherwise we reject it
            var possibleContructors = type.TypeSymbol.Constructors.Where(x => x.Parameters.Length == 0 || x.Parameters.Length == 1);
            if (!possibleContructors.Any(x => x.Parameters.Length != 1 || IsFirstParamParserDiscover(x)) {
                context.ReportDiagnostic();
                return false;
            }
            var formatInfo = GetFormatInfo(type);
            formatInfo.IsSealed |= !possibleContructors.Any(x => x.Parameters.Length == 1 && IsFirstParamParserDiscover(x));
            // Wrap around a parser provider if not sealed

            bool IsFirstParamParserDiscover(IMethodSymbol method) => method.Parameters[0].Type.Equals(type.Analyzer.GetSymbol<IParserDiscovery>(), SymbolEqualityComparer.Default));
        }
    }
}
