using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics.Internal {
    internal interface IConditionProvider : IConditionRules {
        ConditionDetail ProvideCondition(AttributeData data);
    }

}
