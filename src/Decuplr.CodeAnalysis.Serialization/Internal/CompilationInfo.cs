using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class CompilationInfo : ICompilationInfo {

        private Exception ArgNull(string arg) => new InvalidOperationException($"Unable to get {arg} because it was not initiliazed yet");
        private Exception AlreadyInit(string arg) => new InvalidOperationException($"{arg} is already initialized and cannot be re-initiliazed");

        private Compilation? _compilation;
        private IEnumerable<TypeDeclarationSyntax>? _syntaxes;

        public Compilation SourceCompilation {
            get => _compilation ?? throw ArgNull(nameof(SourceCompilation));
            set {
                if (_compilation != null)
                    throw AlreadyInit(nameof(SourceCompilation));
                _compilation = value;
            }
        }

        public IEnumerable<TypeDeclarationSyntax> DeclarationSyntaxes {
            get => _syntaxes ?? throw ArgNull(nameof(DeclarationSyntaxes));
            set {
                if (_syntaxes != null)
                    throw AlreadyInit(nameof(DeclarationSyntaxes));
                _syntaxes = value;
            }
        }

    }
}
