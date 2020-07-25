using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis {
    /// <summary>
    /// Provides a unique and valid symbol name from a symbol
    /// </summary>
    public interface IUniqueNameProvider {
        /// <summary>
        /// Generates a unique and valid symbol name for the symbol
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <returns>A unique name</returns>
        string GetUniqueName(ISymbol symbol);
    }
}
