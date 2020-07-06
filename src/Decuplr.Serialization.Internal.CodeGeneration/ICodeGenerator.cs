using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.CodeGeneration {
    public interface ICodeGenerator {
        ISourceGeneratedResult Validate(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, CancellationToken ct);
    }
}