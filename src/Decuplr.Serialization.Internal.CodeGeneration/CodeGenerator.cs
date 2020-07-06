using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.CodeGeneration {

    internal class FormattingFeatureProvider : IFormattingFeature {

        public IEnumerable<IConditionValidatable> ConditionValidatables { get; }

        public IFormattingFeature AddConditionResolver<TResolver>() where TResolver : IConditionResolverProvider, new() {
            throw new System.NotImplementedException();
        }

        public IFormattingFeature AddFormatResolver<TResolver>() where TResolver : IFormatResolverProvider, new() {
            throw new System.NotImplementedException();
        }
    }

    internal class CodeGenerator : ICodeGenerator {

        private class FaultedGeneratedResult : ISourceGeneratedResult {

            public IEnumerable<Diagnostic> Diagnostics { get; }

            public bool IsFaulted => true;

            public FaultedGeneratedResult(IEnumerable<Diagnostic> diagnostics) {
                Diagnostics = diagnostics;
            }

            public string GenerateFiles() => throw new InvalidOperationException("Faulted result cannot produce correct files");
        }

        private static readonly SymbolKind[] ValidSerializeKind = new [] { SymbolKind.Field, SymbolKind.Property };
        private readonly IReadOnlyList<IGeneratorProvider> _providers;

        internal CodeGenerator(IReadOnlyList<IGeneratorProvider> providers) {
            _providers = providers;
        }

        public ISourceGeneratedResult Validate(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, CancellationToken ct) {
            var analysis = new SourceCodeAnalysis(declarationSyntaxes, compilation, ct, SymbolKind.Method, SymbolKind.Field, SymbolKind.Property, SymbolKind.Event);
            foreach (var type in analysis.ContainingTypes) {
                // See if any provider wants this type, if not we move on to the next
                if (!TryElectProvider(type, out var provider, out var schema))
                    continue;
                var validator = new TypeValidator(type, schema);
                var features = new FormattingFeatureProvider();

                // Allow the provider (BinaryFormat) to configure certain features (IgnoreIf, BitUnion)
                provider.ConfigureFeatures(features);
                
                // First provide the validator to each items that need verifying
                foreach(var validatable in features.ConditionValidatables)
                    validatable.ValidConditions(validator);

                // Then we validate members with our selector and those who request validate them
                "consider making this validator.ValidateLayout(member => ValidSerializeKind.Contains(member.Symbol.Kind), provider.OrderSelector, features.ConditionValidatables, out var layout, out var diagnostics)";
                if (!validator.ValidateLayout(member => ValidSerializeKind.Contains(member.Symbol.Kind), provider.OrderSelector, out var layout, out var diagnostics))
                    return new FaultedGeneratedResult(diagnostics);

            }

            bool TryElectProvider(NamedTypeMetaInfo type, out IGeneratorProvider? electedProvider, out SchemaPrecusor schema) {
                foreach (var provider in _providers) {
                    if (!provider.TryGetSchemaInfo(type, out schema))
                        continue;
                    electedProvider = provider;
                    return true;
                }
                electedProvider = null;
                schema = default;
                return false;
            }
        }
    }

}
