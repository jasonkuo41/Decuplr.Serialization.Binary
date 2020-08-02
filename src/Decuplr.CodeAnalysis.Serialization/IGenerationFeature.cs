namespace Decuplr.CodeAnalysis.Serialization {
    public interface IGenerationFeatures {
        IGenerationFeatures AddConditionResolver<TResolver>() where TResolver : class, IConditionResolverProvider;
        IGenerationFeatures AddFormatResolver<TResolver>() where TResolver : class, IMemberDataFormatterProvider;
        IGenerationFeatures UseSolution<TSolution>() where TSolution : class, IParsingSolution;
    }
}
