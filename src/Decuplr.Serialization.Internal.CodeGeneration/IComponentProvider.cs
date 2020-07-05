using Decuplr.Serialization.CodeGeneration.Arguments;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IComponentProvider {
        string FullTypeName { get; }
        string GetComponent(ParserDiscoveryArgs args);
        string TryGetComponent(ParserDiscoveryArgs args, OutArgs<object> result);
    }
}
