using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.Internal;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Decuplr.CodeAnalysis.Serialization.StartupServices {

    internal class GenerationStartupServiceConfig {

        public IReadOnlyDictionary<IGenerationStartup, IServiceProvider> StartupServices { get; }

        public GenerationStartupServiceConfig(IEnumerable<IGenerationStartup> startups, IReadOnlyCollection<ServiceDescriptor> sourceCollection, IServiceProvider sourceProvider) {
            // Configure service provider foreach startup
            StartupServices = startups.ToDictionary(startup => startup, startup => {
                // Maybe we can make this lazy initialized
                // We include all services provided by the generator to each startup
                var services = new ServiceCollection();
                foreach (var service in sourceCollection) {
                    services.Add(new ServiceDescriptor(service.ServiceType, provider => {
                        if (service.Lifetime == ServiceLifetime.Singleton)
                            return sourceProvider.GetService(service.ServiceType);
                        var sourceService = provider.GetRequiredService<SourceScopeService>().CurrentScopeService;
                        return sourceService.GetService(service.ServiceType);
                    }, service.Lifetime));
                }
                services.AddSingleton<SourceScopeService>();
                services.AddTypeComposite();
                services.AddFeatureProvider(startup);
                services.AddSourceValidation(); // Add ISourceValidation for each startup
                return services.BuildServiceProvider() as IServiceProvider;
            });
        }

    }
}
