using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.CodeAnalysis.Diagnostics.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Decuplr.CodeAnalysis.Diagnostics {
    public static class ValidationExtensions {
        public static IServiceCollection AddSourceValidation(this IServiceCollection services) {
            services.TryAddScoped<ISourceValidation, SourceValidation>();
            services.TryAddScoped<ConditionAnalyzer>();
            services.TryAddScoped<IConditionAnalyzer>(x => x.GetRequiredService<ConditionAnalyzer>());
            return services;
        }
    }
}
