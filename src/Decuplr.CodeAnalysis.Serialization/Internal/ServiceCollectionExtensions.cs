using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal static class ServiceCollectionExtensions {

        public static IServiceCollection AddFeatureProvider(this IServiceCollection collection, IGenerationStartup startup)
            => GeneratorFeaturesProvider.ConfigureServices(startup, collection);

        public static IServiceCollection Add(this IServiceCollection collection, IEnumerable<ServiceDescriptor> descriptors) {
            foreach (var descriptor in descriptors) {
                collection.Add(descriptor);
            }
            return collection;
        }
    }

}
