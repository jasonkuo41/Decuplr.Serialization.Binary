using System;
using System.Collections.Generic;
using Decuplr.Serialization.LayoutService;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    [Obsolete]
    internal static class ContextServiceCollectionExtensions {

        private static IServiceCollection AddScoped<TContext, T>(this IServiceCollection collection, Func<TContext, T?> selector) where T : class {
            return collection.AddScoped(services => selector(services.GetRequiredService<TContext>()) ?? throw new InvalidOperationException("Context is not initialized and cannot be accessed"));
        }

        private static IServiceCollection AddBaseContext<TContext>(this IServiceCollection collection) where TContext : CompilationContext {

            collection.AddScoped<TContext, SourceCodeAnalysis>(x => x.SymbolProvider);
            collection.AddScoped<TContext, ITypeSymbolProvider>(x => x.SymbolProvider);

            collection.AddScoped<TContext, ISourceAddition>(x => x.SourceProvider);
            collection.AddScoped<TContext, IDiagnosticReporter>(x => x.DiagnosticReporter);
            collection.AddScoped<TContext, ICompilationInfo>(x => x.CompilationInfo);

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

}
