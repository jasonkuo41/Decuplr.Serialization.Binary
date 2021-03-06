﻿using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    public interface IComponentProvider {
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
