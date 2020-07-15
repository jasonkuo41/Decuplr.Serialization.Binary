using Decuplr.Serialization.CodeGeneration.Arguments;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IFormatterParsingMethod<TArgs> :
        IChainedMethodBodyProvider<TryDeserializeSpanArgs<TArgs>>,
        IChainedMethodBodyProvider<TryDeserializeSequenceArgs<TArgs>>,
        IChainedMethodBodyProvider<DeserializeSpanArgs<TArgs>>,
        IChainedMethodBodyProvider<DeserializeSequenceArgs<TArgs>>,
        IChainedMethodBodyProvider<SerializeArgs<TArgs>>,
        IChainedMethodBodyProvider<TrySerializeArgs<TArgs>>,
        IChainedMethodBodyProvider<GetLengthArgs<TArgs>> {

        string FormatterName { get; }
    }
}
