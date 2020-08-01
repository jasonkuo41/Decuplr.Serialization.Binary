using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface ICodeGeneratorFactory {
        void RunGeneration(Compilation compilation, IEnumerable<TypeDeclarationSyntax> syntaxes, Action<ICodeGenerator> generatorAction);
    }
}
