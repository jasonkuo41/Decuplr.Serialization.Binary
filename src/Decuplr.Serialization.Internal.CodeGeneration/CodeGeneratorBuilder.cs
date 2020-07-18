using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.CodeGeneration.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Serialization.CodeGeneration {
    public class CodeGeneratorBuilder : ICodeGenerationSourceBuilder, ICodeGenDepenedencyBuilder  {

        private readonly ServiceCollection _services = new ServiceCollection();
        private readonly HashSet<Type> _startups = new HashSet<Type>();

        public ICodeGenerationSourceBuilder AddStartup<TProvider>() where TProvider : class, IGenerationStartup {
            _startups.Add(typeof(IGenerationStartup));
            return this;
        }

        public ICodeGenDepenedencyBuilder UseDependencyProvider<TProvider>() where TProvider : class, IComponentCollection {
            _services.AddSingleton<IComponentCollection, TProvider>();
            return this;
        }

        ICodeGenerator ICodeGenDepenedencyBuilder.CreateGenerator() {
            if (_startups.Count == 0)
                throw new ArgumentException("No entry startup is provided. Code Generation Failed");
            return new CodeGenerator(_services, _startups);
        }
    }

    public interface ICodeGenerationSourceBuilder {
        ICodeGenDepenedencyBuilder UseDependencyProvider<TProvider>() where TProvider : class, IComponentCollection;
        ICodeGenerationSourceBuilder AddStartup<TProvider>() where TProvider : class, IGenerationStartup;
    }

    public interface ICodeGenDepenedencyBuilder {
        ICodeGenDepenedencyBuilder UseDependencyProvider<TProvider>() where TProvider : class, IComponentCollection;
        ICodeGenerator CreateGenerator();
    }

}
