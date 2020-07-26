using System;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    public interface IComponentProvider {
        /// <summary>
        /// The name of the source that we can refer as
        /// </summary>
        string Name { get; }

        /// <summary>
        /// In Decuplr.Serialization.Binary it should be IParserDiscovery
        /// </summary>
        INamedTypeSymbol DiscoveryType { get; }

        /// <summary>
        /// Provides the component type to the composer struct
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        IComponentTypeInfo ProvideComponent(ITypeSymbol component);
    }
}
