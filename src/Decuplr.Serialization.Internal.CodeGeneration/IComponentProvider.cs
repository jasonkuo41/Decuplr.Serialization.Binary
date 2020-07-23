using System;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IComponentProvider {
        /// <summary>
        /// The name of the source that we can refer as
        /// </summary>
        string Name { get; }

        /// <summary>
        /// In Decuplr.Serialization.Binary it should be IParserDiscovery
        /// </summary>
        Type DiscoveryType { get; }

        /// <summary>
        /// Provides the component type to the composer struct
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        IComponentTypeInfo ProvideComponent(ITypeSymbol component);
    }

    public interface ITypeParserDirector : IComponentProvider {
        void CreateTypeParser(SchemaLayout layout, IParserGenerator generator);
    }
}
