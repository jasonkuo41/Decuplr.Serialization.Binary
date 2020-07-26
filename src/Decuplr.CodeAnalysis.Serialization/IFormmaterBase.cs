using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;

namespace Decuplr.CodeAnalysis.Serialization {
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
