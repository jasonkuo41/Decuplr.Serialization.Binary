namespace Decuplr.Serialization.CodeGeneration {
    public interface IFormattingFeature {
        IFormattingFeature AddConditionResolver<TResolver>() where TResolver : IConditionResolverProvider, new();
        IFormattingFeature AddFormatResolver<TResolver>() where TResolver : IFormatResolverProvider, new();
    }
}
