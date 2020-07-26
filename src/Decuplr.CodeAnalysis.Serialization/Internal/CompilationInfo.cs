using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class CompilationInfo : ICompilationInfo {

        public Compilation SourceCompilation { get; }

        public IEnumerable<TypeDeclarationSyntax> DeclarationSyntaxes { get; }

        public CompilationInfo(Compilation sourceCompilation, IEnumerable<TypeDeclarationSyntax> declarationSyntaxes) {
            SourceCompilation = sourceCompilation;
            DeclarationSyntaxes = declarationSyntaxes;
        }

    }
}
