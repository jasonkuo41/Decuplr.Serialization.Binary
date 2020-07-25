using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis {
    /// <summary>
    /// Get's the info of this compilation
    /// </summary>
    public interface ICompilationInfo {

        /// <summary>
        /// All declaring syntaxes in the compilation
        /// </summary>
        IEnumerable<TypeDeclarationSyntax> DeclarationSyntaxes { get; }

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
