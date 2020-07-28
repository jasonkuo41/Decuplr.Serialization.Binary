using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class StartupServiceProvider {

        private readonly Dictionary<IGenerationStartup, IServiceProvider> _startupServices;

        public StartupServiceProvider(IEnumerable<IGenerationStartup> startups, IServiceCollection sourceCollection, IServiceProvider sourceProvider) {
            // Configure service provider foreach startup
            _startupServices = startups.ToDictionary(startup => startup, startup => {
                // Maybe we can make this lazy initialized
                // We include all services provided by the generator to each startup
                var services = new ServiceCollection { sourceCollection.Select(x => new ServiceDescriptor(x.ServiceType, _ => sourceProvider.GetService(x.ServiceType), x.Lifetime)) };
                services.AddFeatureProvider(startup);
                return services.BuildServiceProvider() as IServiceProvider;
            });
        }

        public IServiceProvider this[IGenerationStartup startup] => _startupServices[startup];

        public IServiceProvider GetServiceProvider(IGenerationStartup startup) => _startupServices[startup];

    }
}
