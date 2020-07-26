using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.Internal {

    internal class CodeGenerator : ICodeGenerator {

        private static readonly SymbolKind[] ValidSerializeKind = new[] { SymbolKind.Field, SymbolKind.Property };

        private readonly IReadOnlyDictionary<IGenerationStartup, IServiceProvider> _startups;
        private readonly IServiceCollection _generatorServices;
        private readonly IServiceProvider _generatorProvider;

        internal CodeGenerator(IServiceCollection services) {
            _generatorServices = ConfigureServices(services);
            _generatorProvider = _generatorServices.BuildServiceProvider();
            _startups = ConfigureGenerationStartup(_generatorServices, _generatorProvider);
        }

        private static IReadOnlyDictionary<IGenerationStartup, IServiceProvider> ConfigureGenerationStartup(IServiceCollection sourceCollection, IServiceProvider sourceServices) {
            var startups = sourceServices.GetServices<IGenerationStartup>();
            // Configure service provider foreach startup
            return startups.ToDictionary(startup => startup, startup => {
                // We include all services provided by the generator to each startup
                var services = new ServiceCollection { sourceCollection.Select(x => new ServiceDescriptor(x.ServiceType, _ => sourceServices.GetService(x.ServiceType), x.Lifetime) };
                services.AddFeatureProvider(startup);
                services.AddParsingContext();
                return services.BuildServiceProvider() as IServiceProvider;
            });
        }

        private static IServiceCollection ConfigureServices(IServiceCollection collection) {
            // We add compilation context (and not parsing context) because here scope is considering the compilation as a whole
            collection.AddCompilationContext();

            collection.AddSourceMetaAnalysis();
            collection.AddSourceValidation();

            return collection;
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
            foreach (var type in analysis.AnalyzedTypes) {
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
                => SourceValidation.CreateFrom(type, orderSelector)
                             .AddValidationSource(_startups[source].GetServices<IGroupValidationProvider>())
                             .Where(member => ValidSerializeKind.Contains(member.Symbol.Kind))
                             .TryValidateLayout(diagnostics, out layout);
        }

        public void VerifySyntax() {

            _generatorProvider.GetRequiredService<ISourceValidation>().Validate()
        }

        public void GenerateFiles() {

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
            compileInfo.CompilationInfo = new SchemaCompilationInfo(layouts.Select(x => new SchemaFactory(this, compileInfo, x, ct)));

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
