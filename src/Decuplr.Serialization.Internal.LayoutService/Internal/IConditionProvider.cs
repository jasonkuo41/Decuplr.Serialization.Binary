using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.LayoutService.Internal {
    internal interface IConditionProvider : IConditionRules {
        Condition ProvideCondition(AttributeData data);
    }

}
