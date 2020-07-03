using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Decuplr.Serialization.Analyzer.BinaryFormat;
using Decuplr.Serialization.Binary.AnalysisService;
using Decuplr.Serialization.Binary.Arguments;
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

    internal interface IFunctionProvider<TArgs> {
        string GetFunctionBody(IFuncSource<TArgs> nextFunc, TArgs args);
    }

    internal interface IResolverBase<TArgs> :
        IFunctionProvider<TryDeserializeSpanArgs<TArgs>>,
        IFunctionProvider<TryDeserializeSequenceArgs<TArgs>>,
        IFunctionProvider<DeserializeSpanArgs<TArgs>>,
        IFunctionProvider<DeserializeSequenceArgs<TArgs>>,
        IFunctionProvider<SerializeArgs<TArgs>>,
        IFunctionProvider<TrySerializeArgs<TArgs>> {

        string ResolverName { get; }
    }

    internal interface IConditionResolver : IResolverBase<TypeSourceArgs> { }

    internal interface IConditionResolverProvider {
        void ValidConditions(ITypeValidator validator);
        IConditionResolver GetResolver(MemberMetaInfo member);
    }

    internal interface IParserResolverProvider {
        void ValidConditions(ITypeValidator validator);
        IParserResolver GetResolver(MemberMetaInfo member, IDependencyProvider provider);
    }

    internal interface IParserResolver : IResolverBase<TypeSourceArgs> {
        bool ShouldResolve { get; }
    }

    internal interface IDependencyProvider {
        string GetComponentName(IComponentProvider provider);
    }

    internal interface IComponentProvider {
        string FullTypeName { get; }
        string GetComponent(ParserDiscoveryArgs args);
        string TryGetComponent(ParserDiscoveryArgs args, OutArgs<object> result);
    }

    internal interface IFuncSource<TArgs> {
        string GetNextFunction(TArgs args);
    }
}
