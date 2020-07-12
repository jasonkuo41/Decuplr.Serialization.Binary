using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {

    internal class FaultedSourceGeneratedResults : ISourceGeneratedResults {

        private readonly ServiceScopeCollection _scopes;

        public bool IsFaulted => true;

        public IEnumerable<Diagnostic> Diagnostics { get; }

        public FaultedSourceGeneratedResults(IEnumerable<Diagnostic> diagnostics, ServiceScopeCollection scopes) {
            Diagnostics = diagnostics;
            _scopes = scopes;
        }

        public IEnumerable<GeneratedSourceCode> GenerateFiles() => throw new InvalidOperationException("Faulted results are uncapable of generating file results");

        public void Dispose() => _scopes.Dispose();

    }

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

        private bool TryValidateType(NamedTypeMetaInfo type, IGenerationSource source, SchemaPrecusor schema, ServiceScopeCollection scopes, Dictionary<TypeLayout, IServiceScope> layouts, List<Diagnostic> diagnostics) {
            var isSuccess = false;
            var scope = _generationServices.GetOrAdd(source, key => GeneratorFeaturesProvider.GetServices(key, _serviceCollection, _serviceProvider)).CreateScope();
            scopes.Add(scope);

            // Allow the provider (BinaryFormat) to configure certain features (IgnoreIf, BitUnion)
            if (TypeValidation.CreateFrom(type, schema, source.OrderSelector)
                               .AddValidationSource(scope.ServiceProvider.GetServices<IValidationSource>())
                               .Where(member => ValidSerializeKind.Contains(member.Symbol.Kind))
                               .ValidateLayout(out var layout, out var localDiagnostics)) {
                // If diagnostic contains any error we would cry :(
                Debug.Assert(!localDiagnostics.Any(x => x.Severity == DiagnosticSeverity.Error));
                layouts.Add(layout!, scope);
            }

            diagnostics.AddRange(localDiagnostics);
            return isSuccess;
        }

        public ISourceGeneratedResults Validate(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, CancellationToken ct) {
            var analysis = new SourceCodeAnalysis(declarationSyntaxes, compilation, ct, SymbolKind.Method, SymbolKind.Field, SymbolKind.Property, SymbolKind.Event);
            var diagnostics = new List<Diagnostic>();
            var scopes = new ServiceScopeCollection(analysis.ContainingTypes.Count);
            var layouts = new Dictionary<TypeLayout, IServiceScope>();
            var sources = new Dictionary<TypeLayout, IGenerationSource>();

            try {
                bool isFaulted = false;
                foreach (var type in analysis.ContainingTypes) {
                    if (!TryElectProvider(type, out var source, out var schema))
                        continue;
                    isFaulted |= !TryValidateType(type, source!, schema, scopes, layouts, diagnostics);
                }

                if (isFaulted)
                    return new FaultedSourceGeneratedResults(diagnostics, scopes);

                return layouts.Select(layout => new ResultGenerator(layout.Key, layout.Value, sources[layout.Key])).ToGeneratedResults(diagnostics);
            }
            catch {
                scopes.Dispose();
                throw;
            }
        }

    }

}
