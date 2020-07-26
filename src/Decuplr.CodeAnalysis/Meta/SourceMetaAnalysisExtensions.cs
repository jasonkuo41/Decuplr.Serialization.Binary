using Decuplr.CodeAnalysis.Meta.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Decuplr.CodeAnalysis.Meta {
    public static class SourceMetaAnalysisExtensions {
        /// <summary>
        /// Add <see cref="ISourceMetaAnalysis"/> service to the IServiceCollection
        /// </summary>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddSourceMetaAnalysis(this IServiceCollection services) {
            services.TryAddSingleton<ISourceMetaAnalysis, SourceCodeAnalysis>();
            return services;
        }
    }

}
