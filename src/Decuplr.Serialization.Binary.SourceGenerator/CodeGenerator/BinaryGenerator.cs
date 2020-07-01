using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.AnalysisService;
using Decuplr.Serialization.Binary.CodeGenerator.Arguments;
using Decuplr.Serialization.Binary.LayoutService;
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


    internal interface IConditionResolver {
        void ValidConditions(ITypeValidator validator);

        string GetTryDeserializeSpan(IFuncSource<TryDeserializeSpanArgs> nextFunc, MemberMetaInfo member, TryDeserializeSpanArgs args);
        string GetTryDeserializeSequence(IFuncSource<TryDeserializeSequenceArgs> nextFunc, MemberMetaInfo member, TryDeserializeSequenceArgs args);
        string GetDeserializeSpan(IFuncSource<TrySer>)
    }

    internal interface IFuncSource<TArgs> {
        string GetFunction(TArgs args);
    }

}
