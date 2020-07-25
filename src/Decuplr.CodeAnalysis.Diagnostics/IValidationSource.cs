namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface IValidationSource {
        void ValidConditions(IFluentTypeValidator validator);
    }
}
