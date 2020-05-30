using System;
using System.Linq;

namespace Decuplr.Serialization.Binary.SourceGenerator.Solutions {
    internal class ObserverSerializer : ISerializeSolution {

        private readonly AnalyzedType TypeInfo;

        public ObserverSerializer(AnalyzedType typeInfo) {
            TypeInfo = typeInfo;
        }

        public GeneratedSourceCode[] GetAdditionalFiles() => Array.Empty<GeneratedSourceCode>();

        public GeneratedFormatFunction GetSerializeFunction() {
            var node = new CodeNodeBuilder();
            var outArgsTypes = string.Join(",", Enumerable.Range(0, TypeInfo.MemberFormatInfo.Count).Select(i => $"out {TypeInfo.MemberFormatInfo[i].MemberTypeSymbol} s_{i}"));
            node.AddNode($"private void DeconstructType({TypeInfo.TypeSymbol} value, {outArgsTypes})", node => {
                for(var i = 0; i < TypeInfo.MemberFormatInfo.Count; ++i) {
                    node.AddStatement($"s_{i} = value.{TypeInfo.MemberFormatInfo}");
                }
            });
            return new GeneratedFormatFunction("DeconstructType", node.ToString());
        }
    }
}
