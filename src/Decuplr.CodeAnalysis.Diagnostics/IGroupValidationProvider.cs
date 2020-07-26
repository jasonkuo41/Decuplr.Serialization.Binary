namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface IGroupValidationProvider {
        void ConfigureValidation(IFluentTypeGroupValidator validator);
    }

    public interface ITypeValidationProvider {
        void ConfigureValidation(IFluentMemberValidator validator);
    }
}
