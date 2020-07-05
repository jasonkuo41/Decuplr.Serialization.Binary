using System.Collections.Generic;
using System.Threading;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.CodeGeneration {

    public interface ISourceGeneratedResult {
        IReadOnlyList<Diagnostic> Diagnostics { get; }

        string GenerateFiles();
    }

    public interface IGeneratorProvider {
        SchemaPrecusor ConfigureFeatures(IFormattingFeature provider);
    }

    internal class CodeGenerator : ICodeGenerator {

        private readonly IReadOnlyList<IGeneratorProvider> _providers;

        internal CodeGenerator(IReadOnlyList<IGeneratorProvider> providers) {
            _providers = providers;
        }

        public bool TryValidateSource(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, CancellationToken ct, out ISourceGeneratedResult result) {

        }
    }

    public interface IFormattingFeature {
        IFormattingFeature AddConditionResolver<TResolver>() where TResolver : IConditionResolverProvider, new();
        IFormattingFeature AddFormatResolver<TResolver>() where TResolver : IFormatResolverProvider, new();
    }

    public interface IFunctionProvider<TArgs> {
        string GetFunctionBody(IFunctionSource<TArgs> nextFunc, TArgs args);
    }

    public interface IResolverBase<TArgs> :
        IFunctionProvider<TryDeserializeSpanArgs<TArgs>>,
        IFunctionProvider<TryDeserializeSequenceArgs<TArgs>>,
        IFunctionProvider<DeserializeSpanArgs<TArgs>>,
        IFunctionProvider<DeserializeSequenceArgs<TArgs>>,
        IFunctionProvider<SerializeArgs<TArgs>>,
        IFunctionProvider<TrySerializeArgs<TArgs>> {

        string ResolverName { get; }
    }

    public interface IConditionResolver : IResolverBase<TypeSourceArgs> { }

    public interface IConditionResolverProvider {
        void ValidConditions(ITypeValidator validator);
        IConditionResolver GetResolver(MemberMetaInfo member);
    }

    public interface IFormatResolver : IResolverBase<TypeSourceArgs> {
        bool ShouldResolve { get; }
    }

    public interface IFormatResolverProvider {
        void ValidConditions(ITypeValidator validator);
        IFormatResolver GetResolver(MemberMetaInfo member, IDependencyProvider provider);
    }

    public interface IDependencyProvider {
        string GetComponentName(ITypeSymbol symbol);
    }

    public interface IDependencyProviderSource : IDependencyProvider {
        IReadOnlyDictionary<string, IComponentProvider> Components { get; }
    }

    public interface IFunctionSource<TArgs> {
        string GetNextFunction(TArgs args);
    }
}
