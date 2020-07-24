using System;
using System.Collections.Generic;
using Decuplr.Serialization.CodeGeneration.TypeComposite;

namespace Decuplr.Serialization.CodeGeneration {
    [Obsolete]
    public interface IDependencyProviderSource : IComponentCollection {
        IReadOnlyDictionary<string, IComponentProviderObsolete> Components { get; }
    }
}
