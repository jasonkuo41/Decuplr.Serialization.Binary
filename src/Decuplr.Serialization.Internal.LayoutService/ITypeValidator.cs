namespace Decuplr.Serialization.LayoutService {
    public interface ITypeValidator {
        ILayoutMemberValidation AnyMembers { get; }
        ILayoutMemberValidation LayoutMembers { get; }
    }
}