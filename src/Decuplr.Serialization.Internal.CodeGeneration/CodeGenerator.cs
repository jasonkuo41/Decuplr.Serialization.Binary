using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.CodeGeneration {

    internal class CodeGenerator : ICodeGenerator {

        private readonly IReadOnlyList<IGeneratorProvider> _providers;

        internal CodeGenerator(IReadOnlyList<IGeneratorProvider> providers) {
            _providers = providers;
        }

        public ISourceGeneratedResult Validate(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, CancellationToken ct) {

        }
    }

}
