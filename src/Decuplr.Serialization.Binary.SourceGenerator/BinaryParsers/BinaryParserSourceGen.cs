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
        public bool TryGenerateParser(IEnumerable<AnalyzedType> types, SourceGeneratorContext context, out IEnumerable<GeneratedParser>? parser) {
            var interestedType = types.Where(type => type.ContainsAttribute<BinaryParserAttribute>());
            var resultParsers = new List<GeneratedParser>();
            foreach(var type in interestedType) {
                var attribute = type.GetAttributes<BinaryParserAttribute>().FirstOrDefault();
                // If native type and does not accept IParserDiscovery as the only paramater in the constructor
                var isNativeAndSealed = type.InheritFrom<TypeParser>() && !type.TypeSymbol.Constructors.Where(x => x.Parameters.Length == 1).Any(x => x.Parameters[0].Type.Equals(type.Analyzer.GetSymbol<IParserDiscovery>(), SymbolEqualityComparer.Default));
                var formatInfo = new FormatInfo {
                    // Native type 
                    IsSealed = isNativeAndSealed || (attribute.Data.GetNamedArgumentValue<bool>(nameof(BinaryParserAttribute.Sealed)) ?? false),
                    RequestLayout = attribute.Data.GetNamedArgumentValue<BinaryLayout>(nameof(BinaryParserAttribute.Layout)) ?? BinaryLayout.Auto,
                    // We don't provide, "please don't deserialize option"
                    NeverDeserialize = false,
                    // BinaryParser's namespace is provided by `ParserNamespace`
                    TargetNamespaces = type.GetAttributes<BinaryParserNamespaceAttribute>().Select(x => (x.Data.ConstructorArguments[0].Value as string)!).ToArray()
                };

            }
            parser = resultParsers;
            return true;
        }
    }
}
