using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Services {
    public interface IAttributeLayoutProvider {
        /// <summary>
        /// Get's a collection of easy to operate attribute helpers
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <returns>Returns null if <paramref name="symbol"/>'s <see cref="ISymbol.Locations"/> are not all <see cref="Location.IsInSource"/></returns>
        IAttributeCollection? GetAttributes(ISymbol symbol);
    }
}
