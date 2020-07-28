using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    /// <summary>
    /// Get's the info of this compilation
    /// </summary>
    public interface ICompilationInfo {
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
