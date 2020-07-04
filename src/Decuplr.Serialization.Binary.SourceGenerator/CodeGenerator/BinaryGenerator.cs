using System.Collections.Generic;
using System.Threading;
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

    internal interface IFormattingFeature {
        IFormattingFeature AddConditionResolver<TResolver>() where TResolver : IConditionResolverProvider, new();
        IFormattingFeature AddFormatResolver<TResolver>() where TResolver : IFormatResolverProvider, new();
    }

    internal interface IFunctionProvider<TArgs> {
        string GetFunctionBody(IFunctionSource<TArgs> nextFunc, TArgs args);
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

    internal interface IFormatResolver : IResolverBase<TypeSourceArgs> {
        bool ShouldResolve { get; }
    }

    internal interface IFormatResolverProvider {
        void ValidConditions(ITypeValidator validator);
        IFormatResolver GetResolver(MemberMetaInfo member, IDependencyProvider provider);
    }

    internal interface IDependencyProvider {
        string GetComponentName(ITypeSymbol symbol);
    }

    internal interface IDependencyProviderSource : IDependencyProvider {
        IReadOnlyDictionary<string, IComponentProvider> Components { get; }
    }

    internal interface IFunctionSource<TArgs> {
        string GetNextFunction(TArgs args);
    }
}
