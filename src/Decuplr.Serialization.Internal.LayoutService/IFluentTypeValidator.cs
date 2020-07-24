namespace Decuplr.Serialization.LayoutService {
    public interface IFluentTypeValidator {
        IFluentMemberValidator AnyMembers { get; }
        IFluentMemberValidator LayoutMembers { get; }
    }
}