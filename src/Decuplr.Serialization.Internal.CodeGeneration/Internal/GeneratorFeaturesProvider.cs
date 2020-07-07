using System;
using Decuplr.Serialization.LayoutService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Decuplr.Serialization.CodeGeneration.Internal {
    internal class GeneratorFeaturesProvider : IGenerationFeatures {

        private readonly IServiceCollection _services;

        private IServiceProvider GetServiceProvider() => _services.BuildServiceProvider();

        private GeneratorFeaturesProvider(IServiceCollection services) {
            _services = services;
        }

        public IGenerationFeatures AddConditionResolver<TResolver>() where TResolver : class, IConditionResolverProvider {
            _services.AddScoped<TResolver>();
            _services.AddScoped<IConditionResolverProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            _services.AddScoped<IValidationSource, TResolver>(service => service.GetRequiredService<TResolver>());
            return this;
        }

        public IGenerationFeatures AddFormatResolver<TResolver>() where TResolver : class, IFormatResolverProvider {
            _services.AddScoped<TResolver>();
            _services.AddScoped<IFormatResolverProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            _services.AddScoped<IValidationSource, TResolver>(service => service.GetRequiredService<TResolver>());
            return this;
        }

        public static IServiceProvider GetServices(IGenerationSource provider, IServiceCollection serviceDescriptors) {
            var featureProvider = new GeneratorFeaturesProvider(new ServiceCollection { serviceDescriptors });
            provider.ConfigureFeatures(featureProvider);
            return featureProvider.GetServiceProvider();
        }
    }

}
