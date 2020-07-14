using System;
using System.Linq;
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

        public IGenerationFeatures AddFormatResolver<TResolver>() where TResolver : class, IMemberDataFormatterProvider {
            _services.AddScoped<TResolver>();
            _services.AddScoped<IMemberDataFormatterProvider, TResolver>(service => service.GetRequiredService<TResolver>());
            _services.AddScoped<IValidationSource, TResolver>(service => service.GetRequiredService<TResolver>());
            return this;
        }

        public static IServiceProvider GetServices(IGenerationSource source, IServiceCollection collection, IServiceProvider provider) {
            var featureProvider = new GeneratorFeaturesProvider(new ServiceCollection { collection.Select(descriptor => ReplaceDescriptorSource(descriptor)) });
            source.ConfigureFeatures(featureProvider);
            return featureProvider.GetServiceProvider();

            ServiceDescriptor ReplaceDescriptorSource(ServiceDescriptor descriptor) => new ServiceDescriptor(descriptor.ServiceType, _ => provider.GetService(descriptor.ServiceType), descriptor.Lifetime);
        }
    }

}
