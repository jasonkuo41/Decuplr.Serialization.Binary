using System.Collections.Generic;
using Decuplr.Serialization.Analyzer.BinaryFormat;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal static class TypeFormatLayoutExtensions {

        static IEnumerable<string> GetMember(this TypeFormatLayout layout, bool containType) {
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

        public static string GetTypeParserContructor(this TypeFormatLayout layout) => string.Join(",", layout.GetMember(true));
        public static string GetTypeParserContructorInvokeParams(this TypeFormatLayout layout) => string.Join(",", layout.GetMember(false));
    }

}
