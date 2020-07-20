using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {

    internal class ParsingContext {

        private static Exception NotInitialized => new InvalidOperationException("Context is not initialized and cannot be accessed");
        private static ParsingContext GetThis(IServiceProvider service) => service.GetRequiredService<ParsingContext>();

        public SourceCodeAnalysis? SymbolProvider { get; set; }
        public TypeLayout? CurrentType { get; set; }
        public ISourceAddition? SourceProvider { get; set; }
        public IDiagnosticReporter? DiagnosticReporter { get; set; }

        public static IServiceCollection AddParsingContext(IServiceCollection collection) {
            collection.AddScoped<ParsingContext>();

            collection.AddScoped(services => GetThis(services).SymbolProvider ?? throw NotInitialized);
            collection.AddScoped<ITypeSymbolProvider>(services => GetThis(services).SymbolProvider ?? throw NotInitialized);

            collection.AddScoped(services => GetThis(services).CurrentType ?? throw NotInitialized);
            collection.AddScoped(services => GetThis(services).SourceProvider ?? throw NotInitialized);
            collection.AddScoped(services => GetThis(services).DiagnosticReporter ?? throw NotInitialized);

            return collection;
        }

    }

    internal class CodeGenerator : ICodeGenerator {

        private struct LayoutConfigurator {
            public TypeLayout Layout { get; set; }
            public IGenerationStartup Startup { get; set; }
            public SchemaConfig SchemaConfig { get; set; }
            public void Deconstruct(out TypeLayout layout, out IGenerationStartup startup, out SchemaConfig config) {
                layout = Layout;
                startup = Startup;
                config = SchemaConfig;
            }
        }

        private static readonly SymbolKind[] ValidSerializeKind = new[] { SymbolKind.Field, SymbolKind.Property };

        private readonly IReadOnlyDictionary<IGenerationStartup, IServiceProvider> _startups;
        private readonly IServiceCollection _startupServices;
        private readonly IServiceProvider _startupProvider;

        internal CodeGenerator(IServiceCollection services, IEnumerable<Type> startups) {
            _startupServices = ParsingContext.AddParsingContext(services);
            _startupProvider = _startupServices.BuildServiceProvider();
            _startups = GetStartups(services, _startupProvider, startups);

            static IReadOnlyDictionary<IGenerationStartup, IServiceProvider> GetStartups(IServiceCollection sourceCollection, IServiceProvider services, IEnumerable<Type> startups)
                => startups.Select(type => (IGenerationStartup)ActivatorUtilities.CreateInstance(services, type))
                           .ToDictionary(key => key, value => GeneratorFeaturesProvider.GetServices(value, sourceCollection, services));
        }

        private bool TryElectProvider(NamedTypeMetaInfo type, out IReadOnlyCollection<(IGenerationStartup Startup, SchemaConfig Config)> electedProvider) {
            var providers = new List<(IGenerationStartup, SchemaConfig)>();
            foreach (var provider in _startups.Keys) {
                if (!provider.TryGetSchemaInfo(type, out var schema))
                    continue;
                providers.Add((provider, schema));
            }
            electedProvider = providers;
            return electedProvider.Count != 0;
        }

        private bool TryValidateType(NamedTypeMetaInfo type, IGenerationStartup source, SchemaConfig schema, IDiagnosticReporter diagnostics, out TypeLayout? layout) {
            var isSuccess = false;

            // Allow the provider (BinaryFormat) to configure certain features (IgnoreIf, BitUnion)
            if (TypeValidation.CreateFrom(type, schema, source.OrderSelector)
                               .AddValidationSource(_startups[source].GetServices<IValidationSource>())
                               .Where(member => ValidSerializeKind.Contains(member.Symbol.Kind))
                               .ValidateLayout(out layout, out var localDiagnostics)) {
                // If diagnostic contains any error we would cry :(
                Debug.Assert(!localDiagnostics.Any(x => x.Severity == DiagnosticSeverity.Error));
                isSuccess = true;
            }

            diagnostics.ReportDiagnostic(localDiagnostics);
            return isSuccess;
        }

        private SourceCodeAnalysis GetSourceCodeAnalysis(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, CancellationToken ct)
            => new SourceCodeAnalysis(declarationSyntaxes, compilation, ct, SymbolKind.Method, SymbolKind.Field, SymbolKind.Property, SymbolKind.Event);

        private bool VerifySyntax(SourceCodeAnalysis analysis, IDiagnosticReporter diagnostics, List<LayoutConfigurator>? layoutConfigs) {

            bool isSuccess = true;
            foreach (var type in analysis.ContainingTypes) {
                if (!TryElectProvider(type, out var providers))
                    continue;

                foreach (var (startup, schema) in providers) {
                    isSuccess &= TryValidateType(type, startup, schema, diagnostics, out var layout);

                    // Add the results to the dictionary (if available)
                    if (layoutConfigs is null || layout is null)
                        continue;
                    layoutConfigs.Add(new LayoutConfigurator {
                        Layout = layout,
                        Startup = startup,
                        SchemaConfig = schema
                    });
                }
            }

            return isSuccess;
        }

        public void VerifySyntax(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnostics, CancellationToken ct)
            => VerifySyntax(GetSourceCodeAnalysis(declarationSyntaxes, compilation, ct), diagnostics, null);

        public void GenerateFiles(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnostics, ISourceAddition sourceTarget, CancellationToken ct) {
            var analysis = GetSourceCodeAnalysis(declarationSyntaxes, compilation, ct);
            var layouts = new List<LayoutConfigurator>();

            if (!VerifySyntax(analysis, diagnostics, layouts))
                return;

            // Start code generation
            foreach (var (layout, startup, schema) in layouts) {
                using var scope = _startups[startup].CreateScope();

                // Set the context
                var context = scope.ServiceProvider.GetRequiredService<ParsingContext>();
                context.CurrentType = layout;
                context.SourceProvider = sourceTarget;
                context.SymbolProvider = analysis;
                context.DiagnosticReporter = diagnostics;

                // start the madness of code generation!

            }

        }
    }

}
