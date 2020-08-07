using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface IConditionAnalyzer {
        string GetEvalString(string typeArgumentName, NamedTypeMetaInfo type, ConditionDetail condition);
    }
}