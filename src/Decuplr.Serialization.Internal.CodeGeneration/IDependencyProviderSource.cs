using System.Collections.Generic;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IDependencyProviderSource : IDependencyProvider {
        IReadOnlyDictionary<string, IComponentProvider> Components { get; }
    }
}
