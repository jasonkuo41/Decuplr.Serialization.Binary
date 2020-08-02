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
            _services.AddScoped<TResolver>();
            _services.AddScoped<IConditionResolverProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            _services.AddScoped<IGroupValidationProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            return this;
        }

        public IGenerationFeatures AddFormatResolver<TResolver>() where TResolver : class, IMemberDataFormatterProvider {
            _services.AddScoped<TResolver>();
            _services.AddScoped<IMemberDataFormatterProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            _services.AddScoped<IGroupValidationProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            return this;
        }

        public IGenerationFeatures UseSolution<TSolution>() where TSolution : class, IParsingSolution {
            _services.AddScoped<TSolution>();
            _services.AddScoped<IParsingSolution, TSolution>(services => services.GetRequiredService<TSolution>());
            _services.AddScoped<IGroupValidationProvider, TSolution>(services => services.GetRequiredService<TSolution>());
            return this;
        }
    }

}
