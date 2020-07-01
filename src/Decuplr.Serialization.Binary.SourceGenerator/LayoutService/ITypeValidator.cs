namespace Decuplr.Serialization.Binary.LayoutService {
    internal interface ITypeValidator {
        ILayoutMemberValidation AnyMembers { get; }
        ILayoutMemberValidation LayoutMembers { get; }
    }
}