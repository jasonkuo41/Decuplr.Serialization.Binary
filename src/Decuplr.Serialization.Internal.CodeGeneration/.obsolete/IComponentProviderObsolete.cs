using Decuplr.Serialization.CodeGeneration.Arguments;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IComponentProviderObsolete {
        string FullTypeName { get; }
        string GetComponent(ParserDiscoveryArgs args);
        string TryGetComponent(ParserDiscoveryArgs args, OutArgs<object> result);
    }
}
