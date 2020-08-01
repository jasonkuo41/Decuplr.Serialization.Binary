using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.StartupServices {
    internal static class StartupSerivceProviderExtensions {
        public static IServiceCollection AddGenerationStartupServices(this IServiceCollection services) {
            services.AddSingleton(provider => ActivatorUtilities.CreateInstance<GenerationStartupServiceConfig>(provider, services as IReadOnlyCollection<ServiceDescriptor>));
            services.AddScoped<IGenerationStartupServices, GenerationStartupServices>();
            return services;
        }
    }
}
