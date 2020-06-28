using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService.Internal {
    internal interface IConditionProvider : IConditionRules {
        Condition ProvideCondition(AttributeData data);
    }

}
