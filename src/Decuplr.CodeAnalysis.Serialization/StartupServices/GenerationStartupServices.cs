using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.StartupServices {
    internal class GenerationStartupServices : IGenerationStartupServices, IDisposable {

        private readonly GenerationStartupServiceConfig _provider;
        private readonly IServiceProvider _scopedService;
        private readonly Dictionary<IGenerationStartup, IServiceScope> scopes = new Dictionary<IGenerationStartup, IServiceScope>();

        public GenerationStartupServices(GenerationStartupServiceConfig provider, IServiceProvider service) {
            _provider = provider;
            _scopedService = service;
        }

        public IServiceProvider GetStartupScopeService(IGenerationStartup startup) {
            if (scopes.TryGetValue(startup, out var scope))
                return scope.ServiceProvider;
            scope = _provider.StartupServices[startup].CreateScope();
            scope.ServiceProvider.GetRequiredService<SourceScopeService>().CurrentScopeService = _scopedService;
            scopes.Add(startup, scope);
            return scope.ServiceProvider;
        }

        public void Dispose() {
            foreach (var scope in scopes.Values)
                scope.Dispose();
        }

    }
}
