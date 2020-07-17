using System;
using System.Collections.Generic;

namespace Decuplr.Serialization.CodeGeneration {
    [Obsolete]
    public interface IDependencyProviderSource : IComponentCollection {
        IReadOnlyDictionary<string, IComponentProvider> Components { get; }
    }
}
