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

    internal readonly struct LayoutConfiguration {
        public TypeLayout Layout { get; }
        public IGenerationStartup Startup { get; }
        public SchemaInfo SchemaInfo { get; }
        public LayoutConfiguration(TypeLayout layout, IGenerationStartup startup, SchemaInfo schemaInfo) {
            Layout = layout;
            Startup = startup;
            SchemaInfo = schemaInfo;
        }

        public void Deconstruct(out TypeLayout layout, out IGenerationStartup startup, out SchemaInfo config) {
            layout = Layout;
            startup = Startup;
            config = SchemaInfo;
        }
    }

    internal class CodeGenerator : ICodeGenerator {

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

        private bool TryElectProvider(NamedTypeMetaInfo type, out IReadOnlyCollection<(IGenerationStartup Startup, SchemaInfo Config, IOrderSelector Selector)> electedProvider) {
            var providers = new List<(IGenerationStartup, SchemaInfo, IOrderSelector)>();
            foreach (var provider in _startups.Keys) {
                if (!provider.TryGetSchemaInfo(type, out var schema, out var selector))
                    continue;
                providers.Add((provider, schema, selector));
            }
            electedProvider = providers;
            return electedProvider.Count != 0;
        }

        private bool TryValidateType(NamedTypeMetaInfo type, IGenerationStartup source, SchemaInfo schema, IDiagnosticReporter diagnostics, IOrderSelector orderSelector, out TypeLayout? layout) {
            var isSuccess = false;

            // Allow the provider (BinaryFormat) to configure certain features (IgnoreIf, BitUnion)
            if (TypeValidation.CreateFrom(type, schema, orderSelector)
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

        private bool VerifySyntax(SourceCodeAnalysis analysis, IDiagnosticReporter diagnostics, List<LayoutConfiguration>? layoutConfigs) {

            bool isSuccess = true;
            foreach (var type in analysis.ContainingTypes) {
                if (!TryElectProvider(type, out var providers))
                    continue;

                foreach (var (startup, schema, orderSelector) in providers) {
                    isSuccess &= TryValidateType(type, startup, schema, diagnostics, orderSelector, out var layout);

                    // Add the results to the dictionary (if available)
                    if (layoutConfigs is null || layout is null)
                        continue;
                    layoutConfigs.Add(new LayoutConfiguration(layout, startup, schema));
                }
            }

            return isSuccess;
        }

        public void VerifySyntax(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnostics, CancellationToken ct)
            => VerifySyntax(GetSourceCodeAnalysis(declarationSyntaxes, compilation, ct), diagnostics, null);

        public void GenerateFiles(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnostics, ISourceAddition sourceTarget, CancellationToken ct) {
            var analysis = GetSourceCodeAnalysis(declarationSyntaxes, compilation, ct);
            var layouts = new List<LayoutConfiguration>();

            if (!VerifySyntax(analysis, diagnostics, layouts))
                return;

            using var generatorScope = _startupProvider.CreateScope();
            var dProvider = generatorScope.ServiceProvider.GetRequiredService<IDependencySourceProvider>();
            // Start code generation
            foreach (var (layout, startup, schema) in layouts) {
                using var startupScope = _startups[startup].CreateScope();
                var startupService = startupScope.ServiceProvider;

                // Set the context
                var context = startupService.GetRequiredService<ParsingContext>();
                context.CurrentType = layout;
                context.SourceProvider = sourceTarget;
                context.SymbolProvider = analysis;
                context.DiagnosticReporter = diagnostics;

                // start the madness of code generation!
                // Get the ISerializeSolution out of the provider
                var solution = startupService.GetRequiredService<ISerializationSolution>();
                dProvider.CreateTypeParser(layout.Type.Symbol, startupScope.ServiceProvider);
            }

        }
    }

}

namespace Decuplr.Serialization.CodeGeneration {

    public interface IRequestedParserCollection : IEnumerable<TypeLayout> {
        
    }

    public readonly struct GeneratedParserInfo {
        public GeneratedParserInfo(string name, string fullName, IReadOnlyList<ITypeSymbol> consumingArguments) {
            Name = name;
            FullName = fullName;
            ConsumingArguments = consumingArguments;
        }

        public string Name { get; }
        public string FullName { get; }
        public IReadOnlyList<ITypeSymbol> ConsumingArguments { get; }
    }
}
