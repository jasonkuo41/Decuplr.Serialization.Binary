namespace Decuplr.Serialization.CodeGeneration {
    public interface IGenerationFeatures {
        IGenerationFeatures AddConditionResolver<TResolver>() where TResolver : class, IConditionResolverProvider;
        IGenerationFeatures AddFormatResolver<TResolver>() where TResolver : class, IFormatResolverProvider;
    }
}
