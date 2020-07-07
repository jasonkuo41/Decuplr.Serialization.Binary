using System;
using System.Linq;
using Decuplr.Serialization.CodeGeneration.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration {
    public class CodeGeneratorBuilder {

        private readonly ServiceCollection _services;

        public CodeGeneratorBuilder AddProvider<TProvider>() where TProvider : class, IGenerationSource {
            _services.AddSingleton<IGenerationSource, TProvider>();
            return this;
        }

        public ICodeGenerator CreateGenerator() {
            if (!_services.Any(x => x.ServiceType == typeof(IGenerationSource)))
                throw new InvalidOperationException("Provider must be provided to generate a binary generator");
            return new CodeGenerator(_services);
        }
    }
}
