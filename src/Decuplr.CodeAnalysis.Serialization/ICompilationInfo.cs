using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    /// <summary>
    /// Get's the info regarding of schema for this compilation
    /// </summary>
    public interface ISchemaCompilationInfo {
        /// <summary>
        /// The schemas that this compiliation would be compiling
        /// </summary>
        IReadOnlyList<ISchemaFactory> CompilingSchemas { get; }

        /// <summary>
        /// Gets all related schema that references the symbol
        /// </summary>
        /// <param name="symbol">The symbol to lookup</param>
        /// <returns></returns>
        IEnumerable<ISchemaFactory> GetSchemaComponents(INamedTypeSymbol symbol);
    }
}
