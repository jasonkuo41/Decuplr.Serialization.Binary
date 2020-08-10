using Decuplr.CodeAnalysis.Meta;
using Decuplr.Serialization;

namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface IConditionAnalyzer {
        string GetEvalString(string typeArgumentName, NamedTypeMetaInfo type, ConditionExpression condition);
    }
}