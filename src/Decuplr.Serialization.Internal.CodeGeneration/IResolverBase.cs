using Decuplr.Serialization.CodeGeneration.Arguments;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IResolverBase<TArgs> :
        IFunctionProvider<TryDeserializeSpanArgs<TArgs>>,
        IFunctionProvider<TryDeserializeSequenceArgs<TArgs>>,
        IFunctionProvider<DeserializeSpanArgs<TArgs>>,
        IFunctionProvider<DeserializeSequenceArgs<TArgs>>,
        IFunctionProvider<SerializeArgs<TArgs>>,
        IFunctionProvider<TrySerializeArgs<TArgs>> {

        string ResolverName { get; }
    }
}
