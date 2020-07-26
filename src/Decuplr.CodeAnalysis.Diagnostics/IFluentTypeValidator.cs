namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface IFluentTypeGroupValidator {
        IFluentMemberValidator AnyMembers { get; }
        IFluentMemberValidator SelectedMembers { get; }
    }
}