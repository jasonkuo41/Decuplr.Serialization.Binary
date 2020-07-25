using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics.Internal {
    internal interface IConditionProvider : IConditionRules {
        Condition ProvideCondition(AttributeData data);
    }

}
