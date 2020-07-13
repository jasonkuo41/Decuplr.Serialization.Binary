using System;
using System.Linq;
using Decuplr.Serialization.CodeGeneration.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration {
    public class CodeGeneratorBuilder : ICodeGenerationSourceBuilder, ICodeGenDepenedencyBuilder  {

        private readonly ServiceCollection _services = new ServiceCollection();

        public ICodeGenerationSourceBuilder AddProvider<TProvider>() where TProvider : class, IGenerationSource {
            _services.AddSingleton<IGenerationSource, TProvider>();
            return this;
        }

        public ICodeGenDepenedencyBuilder UseDependencyProvider<TProvider>() where TProvider : class, IComponentCollection {
            _services.AddSingleton<IComponentCollection, TProvider>();
            return this;
        }

        ICodeGenerator ICodeGenDepenedencyBuilder.CreateGenerator() {
            if (!_services.Any(x => x.ServiceType == typeof(IGenerationSource)))
                throw new InvalidOperationException("Provider must be provided to generate a binary generator");
            return new CodeGenerator(_services);
        }
    }

    public interface ICodeGenerationSourceBuilder {
        ICodeGenDepenedencyBuilder UseDependencyProvider<TProvider>() where TProvider : class, IComponentCollection;
        ICodeGenerationSourceBuilder AddProvider<TProvider>() where TProvider : class, IGenerationSource;
    }

    public interface ICodeGenDepenedencyBuilder {
        ICodeGenDepenedencyBuilder UseDependencyProvider<TProvider>() where TProvider : class, IComponentCollection;
        ICodeGenerator CreateGenerator();
    }

}
