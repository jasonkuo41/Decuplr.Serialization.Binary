using System;
using System.Collections.Generic;
using System.Threading;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.CodeGeneration {
    public interface ICodeGenerator {
        void VerifySyntax(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnosticReporter, CancellationToken ct);
        void GenerateFiles(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnosticReporter, ISourceAddition sourceTarget, CancellationToken ct);
    }
}