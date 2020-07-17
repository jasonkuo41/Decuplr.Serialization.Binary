using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IDependencySourceProvider {
        /// <summary>
        /// The name of the source that we can refer as
        /// </summary>
        string Name { get; }

        /// <summary>
        /// In Decuplr.Serialization.Binary it should be IParserDiscovery
        /// </summary>
        Type DiscoveryType { get; }

        IComponentData ProvideComponent(ITypeSymbol component);
    }
}
