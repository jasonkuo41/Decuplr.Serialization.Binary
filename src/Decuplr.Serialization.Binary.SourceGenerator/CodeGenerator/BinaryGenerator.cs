using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.CodeGenerator {

    internal interface ISourceGeneratedResult {
        IReadOnlyList<Diagnostic> Diagnostics { get; }

        string GenerateFiles();
    }

    internal interface IResolverFeatureProvider {

    }

    internal interface IGeneratorProvider {
        SchemaPrecusor ConfigureFeatures(IResolverFeatureProvider provider);
    }

    internal class BinaryGenerator {

        private readonly IReadOnlyList<IGeneratorProvider> _providers;

        internal BinaryGenerator(IReadOnlyList<IGeneratorProvider> providers) {
            _providers = providers;
        }

        public bool TryValidateSource(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, CancellationToken ct, out ISourceGeneratedResult result) {

        }
    }
}
