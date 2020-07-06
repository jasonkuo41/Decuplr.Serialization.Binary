using Decuplr.Serialization.CodeGeneration.Arguments;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IFormatResolver : IResolverBase<TypeSourceArgs> {
        bool ShouldResolve { get; }
    }
}
