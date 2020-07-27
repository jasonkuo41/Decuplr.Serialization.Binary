using Decuplr.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class GeneratorFeaturesProvider : IGenerationFeatures {

        private readonly IServiceCollection _services;

        private GeneratorFeaturesProvider(IServiceCollection services) {
            _services = services;
        }

        public static IServiceCollection ConfigureServices(IGenerationStartup source, IServiceCollection collection) {
            var featureProvider = new GeneratorFeaturesProvider(collection);
            source.ConfigureFeatures(featureProvider);
            return featureProvider._services;
        }

        public IGenerationFeatures AddConditionResolver<TResolver>() where TResolver : class, IConditionResolverProvider {
            _services.AddSingleton<TResolver>();
            _services.AddSingleton<IConditionResolverProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            _services.AddSingleton<IGroupValidationProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            return this;
        }

        public IGenerationFeatures AddFormatResolver<TResolver>() where TResolver : class, IMemberDataFormatterProvider {
            _services.AddSingleton<TResolver>();
            _services.AddSingleton<IMemberDataFormatterProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            _services.AddSingleton<IGroupValidationProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            return this;
        }

        public IGenerationFeatures UseSolution<TSolution>() where TSolution : class, ISerializationSolution {
            _services.AddSingleton<TSolution>();
            _services.AddSingleton<ISerializationSolution, TSolution>(services => services.GetRequiredService<TSolution>());
            return this;
        }
    }

}
