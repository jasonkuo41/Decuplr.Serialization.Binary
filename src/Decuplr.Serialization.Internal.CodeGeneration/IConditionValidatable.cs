using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IConditionValidatable {
        void ValidConditions(ITypeValidator validator);
    }
}
