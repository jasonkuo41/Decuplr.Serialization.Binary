namespace Decuplr.Serialization.CodeGeneration {
    public interface IGenerationFeatures {
        IGenerationFeatures AddConditionResolver<TResolver>() where TResolver : class, IConditionResolverProvider;
        IGenerationFeatures AddFormatResolver<TResolver>() where TResolver : class, IMemberDataFormatterProvider;

        IGenerationFeatures AddDeserializeSolution<TSolution>() where TSolution : class, IDeserializationSolution;
        IGenerationFeatures AddSerializeSolution<TSolution>() where TSolution : class, ISerializationSolution;
        IGenerationFeatures AddLengthProvider<TProvider>() where TProvider : class, ILengthSolution;
    }
}
