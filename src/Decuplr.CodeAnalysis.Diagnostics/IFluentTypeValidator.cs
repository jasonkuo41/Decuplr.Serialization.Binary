namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface IFluentTypeValidator {
        IFluentMemberValidator AnyMembers { get; }
        IFluentMemberValidator LayoutMembers { get; }
    }
}