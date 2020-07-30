using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    static class CompositeServiceCollectionExtensions {
        /// <summary>
        /// Add this to each individual startup services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTypeComposite(IServiceCollection services) {
            services.AddScoped<TypeComposerBuilder>();
            services.AddScoped<MemberComposerFactory>();
        }
    }
}
