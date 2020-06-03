using System.Collections.Generic;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal static class TypeFormatLayoutExtensions {

        public static IEnumerable<string> GetConstructorMember(this TypeFormatLayout layout, bool containType) {
            for (var i = 0; i < layout.Member.Count; ++i) {
                var annotation = layout.Member[i].DecisionAnnotation;
                for (var j = 0; j < annotation.RequestParserType.Count; ++j) {
                    if (containType)
                        yield return $"{annotation.RequestParserType[j]} parser_{i}_{j}";
                    else
                        yield return $"parser_{i}_{j}";
                }
            }
        }

        public static string GetTypeParserContructor(this TypeFormatLayout layout) => string.Join(",", layout.GetConstructorMember(true));
        public static string GetTypeParserContructorInvokeParams(this TypeFormatLayout layout) => string.Join(",", layout.GetConstructorMember(false));

        public static string GetDefaultParserCollectionName(this TypeFormatLayout layout) => $"{layout.TypeSymbol.ToString().Replace('.', '_') }_TypeParserArgs";

        public static string GetDefaultAssemblyEntryClass(this IAssemblySymbol assemblySymbol) => $"AssemblyFormatProvider_{assemblySymbol.ToString().Replace('.', '_')}";
    }

}
