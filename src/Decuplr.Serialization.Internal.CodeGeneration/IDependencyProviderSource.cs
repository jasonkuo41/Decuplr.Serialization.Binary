using System.Collections.Generic;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IDependencyProviderSource : IComponentCollection {
        IReadOnlyDictionary<string, IComponentProvider> Components { get; }
    }
}
