using System;
using System.Linq;

namespace Decuplr.Serialization.Binary.SourceGenerator.Solutions {

    // We require observer solution to have their constructor having default ones

    internal class ObserverDeserializer : IDeserializeSolution {

        private readonly AnalyzedType TypeInfo;

        public ObserverDeserializer(AnalyzedType typeInfo) {
            TypeInfo = typeInfo;
        }

        // Duplicate code, should we inherit instead?
        private string GetConstructorParameters() => string.Join(",", TypeInfo.MemberFormatInfo.Select(x => $"{x.MemberTypeSymbol} s_{x.MemberSymbol.Name}"));

        public GeneratedSourceCode[] GetAdditionalFiles() => Array.Empty<GeneratedSourceCode>();

        public GeneratedFormatFunction GetDeserializeFunction() {
            var node = new CodeNodeBuilder();
            node.AddNode($"private {TypeInfo.TypeSymbol} CreateType({GetConstructorParameters()})", node => {
                // Must be constructless!, otherwise we will fail
                node.AddStatement($"var target = new {TypeInfo.TypeSymbol}()");
                foreach (var member in TypeInfo.MemberFormatInfo)
                    node.AddStatement($"target.{member.MemberSymbol.Name} = s_{member.MemberSymbol.Name}");
                node.AddStatement($"return target");
            });
            return new GeneratedFormatFunction("CreateType", node.ToString());
        }
    }
}
