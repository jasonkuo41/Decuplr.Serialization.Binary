using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator.Solutions {

    // We require observer solution to have their constructor having default ones

    class ObserverSolution : IDeserializeSolution {

        private readonly TypeFormatInfo TypeInfo;

        public ObserverSolution(TypeFormatInfo typeInfo) {
            TypeInfo = typeInfo;
        }

        // Duplicate code, should we inherit instead?
        private string GetConstructorParameters() => string.Join(",", TypeInfo.Members.Select(x => $"{x.MemberTypeSymbol} s_{x.MemberSymbol.Name}"));

        public GeneratedSourceCode[] GetAdditionalFiles() => Array.Empty<GeneratedSourceCode>();

        public FormattingFunction GetDeserializeFunction() {
            var node = new CodeNodeBuilder();
            node.AddNode($"private {TypeInfo.TypeSymbol} CreateType({GetConstructorParameters()})", node => {
                // Must be constructless!, otherwise we will fail
                node.AddStatement($"var target = new {TypeInfo.TypeSymbol}()");
                foreach(var member in TypeInfo.Members)
                    node.AddStatement($"target.{member.MemberSymbol.Name} = s_{member.MemberSymbol.Name}");
                node.AddStatement($"return target");
            });
            return new FormattingFunction("CreateType", node.ToString());
        }
    }
}
