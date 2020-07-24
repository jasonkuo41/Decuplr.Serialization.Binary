using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Decuplr.Serialization.AnalysisService;
using Decuplr.Serialization.CodeGeneration.TypeComposite;
using Decuplr.Serialization.LayoutService;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration.Internal {

    internal class CodeGenerator : ICodeGenerator {

        private static readonly SymbolKind[] ValidSerializeKind = new[] { SymbolKind.Field, SymbolKind.Property };

        private readonly IReadOnlyDictionary<IGenerationStartup, IServiceProvider> _startups;
        private readonly IServiceCollection _generatorServices;
        private readonly IServiceProvider _generatorProvider;

        internal CodeGenerator(IServiceCollection services, IEnumerable<Type> startups) {
            _generatorServices = ConfigureStartup(services);
            _generatorProvider = _generatorServices.BuildServiceProvider();
            _startups = GetStartups(_generatorServices, _generatorProvider, startups);

            static IReadOnlyDictionary<IGenerationStartup, IServiceProvider> GetStartups(IServiceCollection sourceCollection, IServiceProvider sourceServices, IEnumerable<Type> startups) {
                return startups
                    .Select(type => (IGenerationStartup)ActivatorUtilities.CreateInstance(sourceServices, type))
                    .ToDictionary(startup => startup, startup => {
                        // Configure service provider foreach startup
                        var services = new ServiceCollection { CloneWithRedirect(sourceCollection, sourceServices) };
                        services.AddFeatureProvider(startup);
                        services.AddParsingContext();
                        return services.BuildServiceProvider() as IServiceProvider;
                    });

                // We clone the startup service collection, and redirect some services back to the service provider, but we don't clone scoped services
                // This is justified because the concept of these two kind of services have very different meaning of "Scope"
                // Generator Scope is when a compilation passes through
                // While GenerationStartup scope is when a type passes through
                //
                // Considerations : Should we just remove the cloning? Or should we just clone singleton services.
                //
                static IEnumerable<ServiceDescriptor> CloneWithRedirect(IServiceCollection collection, IServiceProvider sourceServices)
                    => collection.Where(x => x.Lifetime != ServiceLifetime.Scoped)
                                 .Select(x => new ServiceDescriptor(x.ServiceType, _ => sourceServices.GetService(x.ServiceType), x.Lifetime));
            }

            // Configure the startup services
            static IServiceCollection ConfigureStartup(IServiceCollection collection) {
                // We add compilation context (and not parsing context) because here scope is considering the compilation as a whole
                collection.AddCompilationContext();
                return collection;
            }
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

        private SourceCodeAnalysis GetSourceCodeAnalysis(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, CancellationToken ct)
            => new SourceCodeAnalysis(declarationSyntaxes, compilation, ct, SymbolKind.Method, SymbolKind.Field, SymbolKind.Property, SymbolKind.Event);

        private bool VerifySyntax(SourceCodeAnalysis analysis, IDiagnosticReporter diagnostics, List<TypeLayoutInfo>? layoutConfigs) {

            bool isSuccess = true;
            foreach (var type in analysis.ContainingTypes) {
                if (!TryElectProvider(type, out var providers))
                    continue;

                foreach (var (startup, schema, orderSelector) in providers) {
                    isSuccess &= TryValidate(type, orderSelector, startup, out var layout);

                    // Add the results to the dictionary (if available)
                    if (layoutConfigs is null || layout is null)
                        continue;
                    layoutConfigs.Add(new TypeLayoutInfo(layout, startup, schema));
                }
            }

            return isSuccess;

            bool TryValidate(NamedTypeMetaInfo type, IOrderSelector orderSelector, IGenerationStartup source, out SchemaLayout? layout)
                => TypeValidation.CreateFrom(type, orderSelector)
                             .AddValidationSource(_startups[source].GetServices<IValidationSource>())
                             .Where(member => ValidSerializeKind.Contains(member.Symbol.Kind))
                             .TryValidateLayout(diagnostics, out layout);
        }

        public void VerifySyntax(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnostics, CancellationToken ct)
            => VerifySyntax(GetSourceCodeAnalysis(declarationSyntaxes, compilation, ct), diagnostics, null);

        public void GenerateFiles(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnostics, ISourceAddition sourceTarget, CancellationToken ct) {
            var analysis = GetSourceCodeAnalysis(declarationSyntaxes, compilation, ct);
            var layouts = new List<TypeLayoutInfo>();

            if (!VerifySyntax(analysis, diagnostics, layouts))
                return;

            using var generatorScope = _generatorProvider.CreateScope();
            var provider = generatorScope.ServiceProvider;

            var compileInfo = provider.GetRequiredService<CompilationContext>();
            compileInfo.DiagnosticReporter = diagnostics;
            compileInfo.SourceProvider = sourceTarget;
            compileInfo.SymbolProvider = analysis;
            compileInfo.CompilationInfo = new CompilationInfo(layouts.Select(x => new SchemaFactory(this, compileInfo, x, ct)));

            provider.GetRequiredService<ITypeParserDirector>().ComposeParser(ct);
        }

        private class SchemaFactory : ISchemaFactory {
            private readonly CodeGenerator _parent;
            private readonly CompilationContext _compilationContext;
            private readonly CancellationToken _ct;

            internal IGenerationStartup Startup { get; }

            public SchemaLayout Layout { get; }

            public SchemaInfo Info { get; }

            public SchemaFactory(CodeGenerator generator, CompilationContext context, TypeLayoutInfo layoutInfo, CancellationToken ct) {
                (Layout, Startup, Info) = layoutInfo;
                _compilationContext = context;
                _parent = generator;
                _ct = ct;
            }

            public GeneratedParserInfo ComposeSchema(string parserName, INamedTypeSymbol type, IComponentProvider componentProvider) {

                if (!Info.TargetTypes.Contains(type, SymbolEqualityComparer.Default))
                    throw new ArgumentException($"'{Layout.Type.Symbol}' layout provided by '{Startup.Name}' does not contain any target type '{type}'");
                
                using var startupScope = _parent._startups[Startup].CreateScope();
                var startupService = startupScope.ServiceProvider;

                // Set the context
                startupService.GetRequiredService<ParsingContext>().SetContext(Layout, _compilationContext);

                // start the madness of code generation!
                var solution = startupService.GetRequiredService<ISerializationSolution>();
                return solution.Generate(componentProvider, Layout, type, _ct);
            }
        }

    }
}
