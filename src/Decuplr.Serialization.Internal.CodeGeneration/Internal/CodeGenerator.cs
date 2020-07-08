using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {

    internal class CodeGenerator : ICodeGenerator {

        private static readonly SymbolKind[] ValidSerializeKind = new[] { SymbolKind.Field, SymbolKind.Property };
        private readonly Dictionary<IGenerationSource, IServiceProvider> _generationServices = new Dictionary<IGenerationSource, IServiceProvider>();

        private readonly List<IGenerationSource> _generationSource;
        private readonly IServiceCollection _serviceCollection;
        private readonly IServiceProvider _serviceProvider;

        internal CodeGenerator(IServiceCollection services) {
            _serviceCollection = services;
            _serviceProvider = services.BuildServiceProvider();
            _generationSource = _serviceProvider.GetServices<IGenerationSource>().ToList();
        }

        private bool TryElectProvider(NamedTypeMetaInfo type, out IGenerationSource? electedProvider, out SchemaPrecusor schema) {
            foreach (var provider in _generationSource) {
                if (!provider.TryGetSchemaInfo(type, out schema))
                    continue;
                electedProvider = provider;
                return true;
            }
            electedProvider = null;
            schema = default;
            return false;
        }

        private GeneratedResult? GenerateResults(NamedTypeMetaInfo type) {
            // See if any provider wants this type, if not we move on to the next
            if (!TryElectProvider(type, out var provider, out var schema))
                return null;

            var scope = _generationServices.GetOrAdd(provider!, key => GeneratorFeaturesProvider.GetServices(key, _serviceCollection)).CreateScope();

            try {
                // Allow the provider (BinaryFormat) to configure certain features (IgnoreIf, BitUnion)
                if (!TypeValidation.CreateFrom(type, schema, provider!.OrderSelector)
                                   .AddValidationSource(scope.ServiceProvider.GetServices<IValidationSource>())
                                   .Where(member => ValidSerializeKind.Contains(member.Symbol.Kind))
                                   .ValidateLayout(out var layout, out var diagnostics)) {

                    return new FaultedGeneratedResult(scope, diagnostics);
                }

                // hand over to the actual generation
                return new ResultGenerator(scope, diagnostics, provider, layout!);
            }
            catch {
                scope.Dispose();
                throw;
            }
        }

        public ISourceGeneratedResults Validate(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, CancellationToken ct) {
            var analysis = new SourceCodeAnalysis(declarationSyntaxes, compilation, ct, SymbolKind.Method, SymbolKind.Field, SymbolKind.Property, SymbolKind.Event);

            return analysis.ContainingTypes.Select(type => GenerateResults(type)).ToGeneratedResults();
        }

    }
}
