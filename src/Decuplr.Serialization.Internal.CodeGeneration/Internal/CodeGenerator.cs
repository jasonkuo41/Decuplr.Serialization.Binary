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

        public static IServiceCollection AddParsingContext(IServiceCollection collection) {
            collection.AddScoped<ParsingContext>();

            collection.AddScoped(services => GetThis(services).SymbolProvider ?? throw NotInitialized);
            collection.AddScoped<ITypeSymbolProvider>(services => GetThis(services).SymbolProvider ?? throw NotInitialized);

            collection.AddScoped(services => GetThis(services).CurrentType ?? throw NotInitialized);
            collection.AddScoped(services => GetThis(services).SourceProvider ?? throw NotInitialized);

            return collection;
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

        private bool TryElectProvider(NamedTypeMetaInfo type, out IReadOnlyCollection<IGenerationStartup> electedProvider, out SchemaConfig schema) {
            foreach (var provider in _startups.Keys) {
                if (!provider.TryGetSchemaInfo(type, out schema))
                    continue;
                electedProvider = provider;
                return true;
            }
            electedProvider = Array.Empty<IGenerationStartup>();
            schema = default;
            return false;
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

        private bool VerifySyntax(SourceCodeAnalysis analysis, IDiagnosticReporter diagnostics, IDictionary<TypeLayout, IGenerationStartup>? layouts) {

            bool isSuccess = true;
            foreach (var type in analysis.ContainingTypes) {
                if (!TryElectProvider(type, out var source, out var schema))
                    continue;

                isSuccess &= TryValidateType(type, source!, schema, diagnostics, out var layout);
                layouts?.Add(layout!, source!);
            }

            return isSuccess;
        }

        public void VerifySyntax(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnostics, CancellationToken ct)
            => VerifySyntax(GetSourceCodeAnalysis(declarationSyntaxes, compilation, ct), diagnostics, null);

        public void GenerateFiles(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnostics, ISourceAddition sourceTarget, CancellationToken ct) {
            var analysis = GetSourceCodeAnalysis(declarationSyntaxes, compilation, ct);
            var layouts = new Dictionary<TypeLayout, IGenerationStartup>();

            if (!VerifySyntax(analysis, diagnostics, layouts))
                return;

            // Start code generation
            foreach (var (layout, startup) in layouts) {
                using var scope = _startups[startup].CreateScope();

                // Set the context
                var context = scope.ServiceProvider.GetRequiredService<ParsingContext>();
                context.CurrentType = layout;
                context.SourceProvider = sourceTarget;
                context.SymbolProvider = analysis;
            }

        }
    }

}
