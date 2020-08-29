using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public interface IComponentResolver {
        ComponentConvertInfo GetComponent(ITypeSymbol source);
    }

    public struct ComponentConvertInfo {
        /// <summary>
        /// The symbol of the converter
        /// </summary>
        public ITypeSymbol ComponentConverter { get; } 

        /// <summary>
        /// If the component has fixed length
        /// </summary>
        public int? FixedBinaryLength { get; }
    }
}
