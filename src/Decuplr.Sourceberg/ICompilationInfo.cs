using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Sourceberg {
    /// <summary>
    /// Get's the info of this compilation
    /// </summary>
    public interface ICompilationInfo {

        /// <summary>
        /// The compilation that was first introduced without modification
        /// </summary>
        Compilation SourceCompilation { get; }

        /// <summary>
        /// All declaring syntaxes in the compilation
        /// </summary>
        IEnumerable<TypeDeclarationSyntax> DeclarationSyntaxes { get; }

    }
}
