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

    internal class CompilationContext {
        public SourceCodeAnalysis? SymbolProvider { get; set; }
        public ISourceAddition? SourceProvider { get; set; }
        public IDiagnosticReporter? DiagnosticReporter { get; set; }

        protected void CopyFrom(CompilationContext context) {
            SymbolProvider = context.SymbolProvider;
            SourceProvider = context.SourceProvider;
            DiagnosticReporter = context.DiagnosticReporter;
        }
    }

    internal class ParsingContext : CompilationContext {

        public SchemaLayout? CurrentLayout { get; set; }

        public void SetContext(SchemaLayout layout, CompilationContext context) {
            CopyFrom(context);
            CurrentLayout = layout;
        }
    }

    internal static class ContextExtensions {

        private static IServiceCollection AddScoped<TContext, T>(this IServiceCollection collection, Func<TContext, T?> selector) where T : class {
            return collection.AddScoped(services => selector(services.GetRequiredService<TContext>()) ?? throw new InvalidOperationException("Context is not initialized and cannot be accessed"));
        }

        private static IServiceCollection AddBaseContext<TContext>(this IServiceCollection collection) where TContext : CompilationContext {

            collection.AddScoped<TContext, SourceCodeAnalysis>(x => x.SymbolProvider);
            collection.AddScoped<TContext, ITypeSymbolProvider>(x => x.SymbolProvider);

            collection.AddScoped<TContext, ISourceAddition>(x => x.SourceProvider);
            collection.AddScoped<TContext, IDiagnosticReporter>(x => x.DiagnosticReporter);

            return collection;
        }

        public static IServiceCollection AddParsingContext(this IServiceCollection collection) {
            collection.AddScoped<ParsingContext>();
            collection.AddScoped<ParsingContext, SchemaLayout>(x => x.CurrentLayout);

            return collection.AddBaseContext<ParsingContext>();
        }

        public static IServiceCollection AddCompilationContext(this IServiceCollection collection) {
            collection.AddScoped<CompilationContext>();
            collection.AddBaseContext<CompilationContext>();
            return collection;
        }

        public static IServiceCollection AddFeatureProvider(this IServiceCollection collection, IGenerationStartup startup) 
            => GeneratorFeaturesProvider.ConfigureServices(startup, collection);

        public static IServiceCollection Add(this IServiceCollection collection, IEnumerable<ServiceDescriptor> descriptors) {
            foreach (var descriptor in descriptors)
                collection.Add(descriptor);
            return collection;
        }
    }

    internal class CodeGenerator : ICodeGenerator {

        private static readonly SymbolKind[] ValidSerializeKind = new[] { SymbolKind.Field, SymbolKind.Property };

        private readonly IReadOnlyDictionary<IGenerationStartup, IServiceProvider> _startups;
        private readonly IServiceCollection _startupServices;
        private readonly IServiceProvider _startupProvider;

        internal CodeGenerator(IServiceCollection services, IEnumerable<Type> startups) {
            _startupServices = ConfigureStartup(services);
            _startupProvider = _startupServices.BuildServiceProvider();
            _startups = GetStartups(_startupServices, _startupProvider, startups);

            static IReadOnlyDictionary<IGenerationStartup, IServiceProvider> GetStartups(IServiceCollection sourceCollection, IServiceProvider sourceServices, IEnumerable<Type> startups) {
                return startups
                    .Select(type => (IGenerationStartup)ActivatorUtilities.CreateInstance(sourceServices, type))
                    .ToDictionary(startup => startup, startup => {
                        // Configure service provider foreach startup
                        var services = new ServiceCollection { CloneRedirect(sourceCollection, sourceServices) };
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
                static IEnumerable<ServiceDescriptor> CloneRedirect(IServiceCollection collection, IServiceProvider sourceServices)
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

        private bool TryValidateType(NamedTypeMetaInfo type, IGenerationStartup source, SchemaInfo schema, IDiagnosticReporter diagnostics, IOrderSelector orderSelector, out SchemaLayout? layout) {
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

        private bool VerifySyntax(SourceCodeAnalysis analysis, IDiagnosticReporter diagnostics, List<TypeLayoutInfo>? layoutConfigs) {

            bool isSuccess = true;
            foreach (var type in analysis.ContainingTypes) {
                if (!TryElectProvider(type, out var providers))
                    continue;

                foreach (var (startup, schema, orderSelector) in providers) {
                    isSuccess &= TryValidateType(type, startup, schema, diagnostics, orderSelector, out var layout);

                    // Add the results to the dictionary (if available)
                    if (layoutConfigs is null || layout is null)
                        continue;
                    layoutConfigs.Add(new TypeLayoutInfo(layout, startup, schema));
                }
            }

            return isSuccess;
        }

        public void VerifySyntax(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnostics, CancellationToken ct)
            => VerifySyntax(GetSourceCodeAnalysis(declarationSyntaxes, compilation, ct), diagnostics, null);

        public void GenerateFiles(IEnumerable<TypeDeclarationSyntax> declarationSyntaxes, Compilation compilation, IDiagnosticReporter diagnostics, ISourceAddition sourceTarget, CancellationToken ct) {
            var analysis = GetSourceCodeAnalysis(declarationSyntaxes, compilation, ct);
            var layouts = new List<TypeLayoutInfo>();

            if (!VerifySyntax(analysis, diagnostics, layouts))
                return;

            using var generatorScope = _startupProvider.CreateScope();
            var provider = generatorScope.ServiceProvider;

            var compileInfo = provider.GetRequiredService<ParsingContext>();

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

    internal readonly struct TypeLayoutInfo {
        public SchemaLayout Layout { get; }

        public IGenerationStartup Startup { get; }

        public SchemaInfo SchemaInfo { get; }

        public TypeLayoutInfo(SchemaLayout layout, IGenerationStartup startup, SchemaInfo schemaInfo) {
            Layout = layout;
            Startup = startup;
            SchemaInfo = schemaInfo;
        }

        public void Deconstruct(out SchemaLayout layout, out IGenerationStartup startup, out SchemaInfo config) {
            layout = Layout;
            startup = Startup;
            config = SchemaInfo;
        }

    }

}

namespace Decuplr.Serialization.CodeGeneration {

    public interface ISchemaFactory {
        SchemaLayout Layout { get; }
        SchemaInfo Info { get; }

        /// <summary>
        /// Generate the target schema
        /// </summary>
        /// <param name="parserName">The name of the generated schema</param>
        /// <param name="type">The target type, must be included in the <see cref="SchemaInfo.TargetTypes"/> </param>
        /// <param name="componentProvider">The provider that generates the component needed for composing the type</param>
        GeneratedParserInfo ComposeSchema(string parserName, INamedTypeSymbol type, IComponentProvider componentProvider);
    }

    /// <summary>
    /// Get's the info of this compilation
    /// </summary>
    public interface ICompilationInfo {
        /// <summary>
        /// Get's the assembly symbol that this compilation takes
        /// </summary>
        IAssemblySymbol CompilingAssembly { get; }

        /// <summary>
        /// The schemas that this compiliation would be compiling
        /// </summary>
        IReadOnlyList<ISchemaFactory> CompilingSchemas { get; }

        /// <summary>
        /// Gets all related schema that references the symbol
        /// </summary>
        /// <param name="symbol">The symbol to lookup</param>
        /// <returns></returns>
        IEnumerable<ISchemaFactory> GetSchemaComponents(INamedTypeSymbol symbol);
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
