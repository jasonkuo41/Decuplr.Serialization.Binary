using Decuplr.Serialization;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Diagnostics.Internal {
    internal interface IConditionProvider : IConditionRules {
        ConditionExpression ProvideCondition(AttributeData data);
    }

}
